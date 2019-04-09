using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SolrNet.Commands.Parameters;
using SolrNet.Utils;

namespace SolrNet.Impl
{
    /// <summary>
    /// ISolrQueryExecuter that POSTs the query request, including any QueryBody
    /// set in the QueryOptions.
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrPostQueryExecuter<T> : AbstractSolrQueryExecuter, ISolrQueryExecuter<T>
    {
        private readonly ISolrConnection connection;
        private readonly ISolrAbstractResponseParser<T> resultParser;
        private readonly ISolrQuerySerializer querySerializer;
        private readonly ISolrMoreLikeThisHandlerQueryResultsParser<T> mlthResultParser;

        public SolrPostQueryExecuter(ISolrAbstractResponseParser<T> resultParser, ISolrConnection connection,
            ISolrQuerySerializer querySerializer, ISolrFacetQuerySerializer facetQuerySerializer,
            ISolrMoreLikeThisHandlerQueryResultsParser<T> mlthResultParser)
            : base(querySerializer, facetQuerySerializer)
        {
            this.connection = connection;
            this.resultParser = resultParser;
            this.querySerializer = querySerializer;
            this.mlthResultParser = mlthResultParser;
        }

        /// <summary>
        /// Return parameters suitable for a JSON POST query.
        /// </summary>
        /// <param name="query">The incoming query, to populate the q parameter.</param>
        /// <param name="options">The query options.</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> GetAllParameters(ISolrQuery query,
            QueryOptions options)
        {
            var q = querySerializer.Serialize(query);
            if (q.Length > 0)
                yield return KV.Create("q", q);

            if (options == null)
                yield break;

            foreach (var p in GetCommonParameters(options))
                yield return p;

            if (options.OrderBy != null && options.OrderBy.Count > 0)
                yield return KV.Create("sort", string.Join(",", options.OrderBy.Select(x => x.ToString()).ToArray()));

            foreach (var p in GetHighlightingParameters(options))
                yield return p;

            foreach (var p in GetSpellCheckingParameters(options))
                yield return p;

            foreach (var p in GetTermsParameters(options))
                yield return p;

            if (options.MoreLikeThis != null)
            {
                foreach (var p in GetMoreLikeThisParameters(options.MoreLikeThis))
                    yield return p;
            }

            foreach (var p in GetStatsQueryOptions(options))
                yield return p;

            foreach (var p in GetCollapseQueryOptions(options))
                yield return p;

            foreach (var p in GetTermVectorQueryOptions(options))
                yield return p;

            foreach (var p in GetGroupingQueryOptions(options))
                yield return p;

            foreach (var p in GetCollapseExpandOptions(options.CollapseExpand, querySerializer.Serialize))
                yield return p;

            foreach (var p in GetClusteringParameters(options))
                yield return p;
        }

        public override IEnumerable<KeyValuePair<string, string>> GetAllMoreLikeThisHandlerParameters(
            SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            var qq = query.Switch(
                query: q => KV.Create("q", querySerializer.Serialize(q)),
                streamBody: body => new KeyValuePair<string, string>(string.Empty, string.Empty),
                streamUrl: url => KV.Create("stream.url", url.ToString()));
            if (!qq.Key.Equals(string.Empty))
                yield return qq;

            if (options == null)
                yield break;

            foreach (var p in GetCommonParameters(options))
                yield return p;

            foreach (var p in GetMoreLikeThisHandlerParameters(options.Parameters))
                yield return p;
        }

        public SolrQueryResults<T> Execute(ISolrQuery q, QueryOptions options)
        {
            var handler = options?.RequestHandler?.HandlerUrl ?? DefaultHandler;
            var param = GetAllParameters(q, options);
            var body = options?.QueryBody?.serialize() ?? String.Empty;
            var results = new SolrQueryResults<T>();
            var r = connection.PostStream(handler, options?.QueryBody?.mimeType ?? SimpleJsonQueryBody.ApplicationJson,
                new MemoryStream(Encoding.UTF8.GetBytes(body)), param);
            var xml = XDocument.Parse(r);
            resultParser.Parse(xml, results);
            return results;
        }

        public async Task<SolrQueryResults<T>> ExecuteAsync(ISolrQuery q, QueryOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var handler = options?.RequestHandler?.HandlerUrl ?? DefaultHandler;
            var param = GetAllParameters(q, options);
            var results = new SolrQueryResults<T>();
            var body = options?.QueryBody?.serialize() ?? String.Empty;

            XDocument xml;
            if (connection is IStreamSolrConnection cc)
            {
                using (var r = await cc.PostStreamAsStreamAsync(handler,
                    options?.QueryBody.mimeType ?? SimpleJsonQueryBody.ApplicationJson,
                    new MemoryStream(Encoding.UTF8.GetBytes(body)), param, cancellationToken))
                {
                    xml = XDocument.Load(r);
                }
            }
            else
            {
                var r = await connection.PostStreamAsync(handler,
                    options?.QueryBody?.mimeType ?? SimpleJsonQueryBody.ApplicationJson,
                    new MemoryStream(Encoding.UTF8.GetBytes(body)), param);
                xml = XDocument.Parse(r);
            }

            resultParser.Parse(xml, results);
            return results;
        }

        public SolrMoreLikeThisHandlerResults<T> Execute(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            var param = GetAllMoreLikeThisHandlerParameters(query, options).ToList();
            var body = GetMoreLikeThisBodyContent(query, options);
            var r = connection.PostStream(MoreLikeThisHandler, body.mimeType,
                new MemoryStream(Encoding.UTF8.GetBytes(body.serialize())), param);
            var qr = mlthResultParser.Parse(r);
            return qr;
        }

        public async Task<SolrMoreLikeThisHandlerResults<T>> ExecuteAsync(SolrMLTQuery query,
            MoreLikeThisHandlerQueryOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var param = GetAllMoreLikeThisHandlerParameters(query, options).ToList();
            var body = GetMoreLikeThisBodyContent(query, options);
            var r = await connection.PostStreamAsync(MoreLikeThisHandler, body.mimeType,
                new MemoryStream(Encoding.UTF8.GetBytes(body.serialize())), param);
            var qr = mlthResultParser.Parse(r);
            return qr;
        }

        private ISolrQueryBody GetMoreLikeThisBodyContent(SolrMLTQuery query,
            MoreLikeThisHandlerQueryOptions options)
        {
            if (options?.QueryBody != null)
            {
                return options.QueryBody;
            }
            else if (query is SolrMoreLikeThisHandlerStreamBodyQuery bodyQuery)
            {
                return new PlainTextQueryBody(bodyQuery.Body);
            }
            else
            {
                return new PlainTextQueryBody(string.Empty);
            }
        }
    }
}
