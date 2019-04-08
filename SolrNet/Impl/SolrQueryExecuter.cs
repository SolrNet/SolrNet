#region license

// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SolrNet.Commands.Parameters;
using SolrNet.Utils;

namespace SolrNet.Impl {
    /// <summary>
    /// Executes queries
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrQueryExecuter<T> : AbstractSolrQueryExecuter, ISolrQueryExecuter<T> {
        private readonly ISolrAbstractResponseParser<T> resultParser;
        private readonly ISolrMoreLikeThisHandlerQueryResultsParser<T> mlthResultParser;
        private readonly ISolrConnection connection;
        private readonly ISolrQuerySerializer querySerializer;

        /// <summary>
        /// Default Solr query handler
        /// </summary>
        public string DefaultHandler { get; set; } = "/select";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultParser"></param>
        /// <param name="connection"></param>
        /// <param name="querySerializer"></param>
        /// <param name="facetQuerySerializer"></param>
        /// <param name="mlthResultParser"></param>
        public SolrQueryExecuter(ISolrAbstractResponseParser<T> resultParser, ISolrConnection connection, ISolrQuerySerializer querySerializer, ISolrFacetQuerySerializer facetQuerySerializer, ISolrMoreLikeThisHandlerQueryResultsParser<T> mlthResultParser)
            :base(querySerializer, facetQuerySerializer) 
        {
            this.resultParser = resultParser;
            this.mlthResultParser = mlthResultParser;
            this.connection = connection;
            this.querySerializer = querySerializer;
        }

        /// <summary>
        /// Gets Solr parameters for all defined query options
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetAllParameters(ISolrQuery Query, QueryOptions options) {
            yield return KV.Create("q", querySerializer.Serialize(Query));
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

            foreach (var p in GetCollapseExpandOptions(options.CollapseExpand, querySerializer.Serialize))
                yield return p;

            foreach (var p in GetClusteringParameters(options))
                yield return p;
        }

        /// <summary>
        /// Serializes all More Like This handler parameters
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetAllMoreLikeThisHandlerParameters(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options) {
            yield return
                query.Switch<KeyValuePair<string, string>>(
                             query: q => KV.Create("q", querySerializer.Serialize(q)),
                             streamBody: body => KV.Create("stream.body", body),
                             streamUrl: url => KV.Create("stream.url", url.ToString()));

            if (options == null)
                yield break;

            foreach (var p in GetCommonParameters(options))
                yield return p;

            foreach (var p in GetMoreLikeThisHandlerParameters(options.Parameters))
                yield return p;
        }

        /// <summary>
        /// Executes the query and returns results
        /// </summary>
        /// <returns>query results</returns>
        public SolrQueryResults<T> Execute(ISolrQuery q, QueryOptions options) {
            var handler = options?.RequestHandler?.HandlerUrl ?? DefaultHandler;
            var param = GetAllParameters(q, options);
            var results = new SolrQueryResults<T>();
            var r = connection.Get(handler, param);
            var xml = XDocument.Parse(r);
            resultParser.Parse(xml, results);
            return results;
        }

        /// <summary>
        /// Executes a MoreLikeThis handler query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SolrMoreLikeThisHandlerResults<T> Execute(SolrMLTQuery q, MoreLikeThisHandlerQueryOptions options) {
            var param = GetAllMoreLikeThisHandlerParameters(q, options).ToList();
            var r = connection.Get(MoreLikeThisHandler, param);
            var qr = mlthResultParser.Parse(r);
            return qr;
        }

        public async Task<SolrQueryResults<T>> ExecuteAsync(ISolrQuery q, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var handler = options?.RequestHandler?.HandlerUrl ?? DefaultHandler;
            var param = GetAllParameters(q, options);
            var results = new SolrQueryResults<T>();

            XDocument xml;
            if (connection is IStreamSolrConnection  cc)
            {
                using (var r = await cc.GetAsStreamAsync(handler, param, cancellationToken))
                {
                    xml = XDocument.Load(r);
                }
            }
            else
            {
                var r = await connection.GetAsync(handler, param, cancellationToken);
                xml = XDocument.Parse(r);
            }

            resultParser.Parse(xml, results);
            return results;
        }

        public async Task<SolrMoreLikeThisHandlerResults<T>> ExecuteAsync(SolrMLTQuery q, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var param = GetAllMoreLikeThisHandlerParameters(q, options).ToList();
            var r = await connection.GetAsync(MoreLikeThisHandler, param, cancellationToken);
            var qr = mlthResultParser.Parse(r);
            return qr;
        }
    }
}
