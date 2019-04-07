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
        private readonly ISolrConnection _connection;
        private readonly ISolrAbstractResponseParser<T> _resultParser;
        private readonly ISolrQuerySerializer _querySerializer;

        public SolrPostQueryExecuter(ISolrAbstractResponseParser<T> resultParser, ISolrConnection connection,
            ISolrQuerySerializer querySerializer, ISolrFacetQuerySerializer facetQuerySerializer,
            ISolrMoreLikeThisHandlerQueryResultsParser<T> mlthResultParser) 
            : base(querySerializer, facetQuerySerializer)
        {
            this._connection = connection;
            this._resultParser = resultParser;
            this._querySerializer = querySerializer;
        }

        /// <summary>
        /// Return parameters suitable for a JSON POST query.
        /// </summary>
        /// <param name="Query">The incoming Query, to populate the q parameter.</param>
        /// <param name="options">The query options.</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetPostParameters(ISolrQuery Query, QueryOptions options)
        {
            var q = _querySerializer.Serialize(_querySerializer.Serialize(Query));
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

            if (options.MoreLikeThis != null) {
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

            foreach (var p in GetCollapseExpandOptions(options.CollapseExpand, _querySerializer.Serialize))
                yield return p;

            foreach (var p in GetClusteringParameters(options))
                yield return p;
        }

        public SolrQueryResults<T> Execute(ISolrQuery q, QueryOptions options) {
            var handler = options?.RequestHandler?.HandlerUrl ?? DefaultHandler;
            var param = GetPostParameters(q, options);
            var body = options?.QueryBody?.serialize() ?? String.Empty;
            var results = new SolrQueryResults<T>();
            var r = _connection.PostStream(handler, options?.QueryBody?.mimeType ?? SimpleJsonQueryBody.ApplicationJson, new MemoryStream(Encoding.UTF8.GetBytes(body)), param);
            var xml = XDocument.Parse(r);
            _resultParser.Parse(xml, results);
            return results;
        }

        public async Task<SolrQueryResults<T>> ExecuteAsync(ISolrQuery q, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var handler = options?.RequestHandler?.HandlerUrl ?? DefaultHandler;
            var param = GetPostParameters(q, options);
            var results = new SolrQueryResults<T>();
            var body = options?.QueryBody?.serialize() ?? String.Empty;

            XDocument xml;
            if (_connection is IStreamSolrConnection  cc)
            {
                using (var r = await cc.PostStreamAsStreamAsync(handler, options?.QueryBody.mimeType ?? SimpleJsonQueryBody.ApplicationJson, new MemoryStream(Encoding.UTF8.GetBytes(body)), param, cancellationToken))
                {
                    xml = XDocument.Load(r);
                }
            }
            else
            {
                var r = await _connection.PostStreamAsync(handler, options?.QueryBody?.mimeType ?? SimpleJsonQueryBody.ApplicationJson, new MemoryStream(Encoding.UTF8.GetBytes(body)), param);
                xml = XDocument.Parse(r);
            }

            _resultParser.Parse(xml, results);
            return results;
        }

        public SolrMoreLikeThisHandlerResults<T> Execute(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<SolrMoreLikeThisHandlerResults<T>> ExecuteAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
