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

using System;
using System.Collections.Generic;
using System.Globalization;
using SolrNet.Commands.Parameters;
using SolrNet.Utils;

namespace SolrNet.Impl {
    /// <summary>
    /// Executes queries
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrQueryExecuter<T> : ISolrQueryExecuter<T> {

        private readonly ISolrQueryResultParser<T> resultParser;
        private readonly ISolrConnection connection;
        private readonly ISolrQuerySerializer querySerializer;
        private readonly ISolrFacetQuerySerializer facetQuerySerializer;

        /// <summary>
        /// When the row count is not defined, use this row count by default
        /// </summary>
        public int DefaultRows { get; set; }

        public static readonly int ConstDefaultRows = 100000000;

        public SolrQueryExecuter(ISolrQueryResultParser<T> resultParser, ISolrConnection connection, ISolrQuerySerializer querySerializer, ISolrFacetQuerySerializer facetQuerySerializer) {
            this.resultParser = resultParser;
            this.connection = connection;
            this.querySerializer = querySerializer;
            this.facetQuerySerializer = facetQuerySerializer;
            DefaultRows = ConstDefaultRows;
        }

        public KeyValuePair<T1, T2> KVP<T1, T2>(T1 a, T2 b) {
            return new KeyValuePair<T1, T2>(a, b);
        }

        /// <summary>
        /// Gets Solr parameters for all defined query options
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetAllParameters(ISolrQuery Query, QueryOptions Options) {
            yield return KVP("q", querySerializer.Serialize(Query));
            if (Options != null) {
                if (Options.Start.HasValue)
                    yield return KVP("start", Options.Start.ToString());
                var rows = Options.Rows.HasValue ? Options.Rows.Value : DefaultRows;
                yield return KVP("rows", rows.ToString());
                if (Options.OrderBy != null && Options.OrderBy.Count > 0)
                    yield return KVP("sort", Func.Join(",", Options.OrderBy));

                if (Options.Fields != null && Options.Fields.Count > 0)
                    yield return KVP("fl", Func.Join(",", Options.Fields));

                foreach (var p in GetHighlightingParameters(Options))
                    yield return p;

                foreach (var p in GetFilterQueries(Options))
                    yield return p;

                foreach (var p in GetSpellCheckingParameters(Options))
                    yield return p;

                foreach (var p in GetMoreLikeThisParameters(Options))
                    yield return p;

                foreach (var p in GetFacetFieldOptions(Options))
                    yield return p;

                foreach (var p in GetStatsQueryOptions(Options))
                    yield return p;

                foreach (var p in GetCollapseQueryOptions(Options))
                    yield return p;

                if (Options.ExtraParams != null)
                    foreach (var p in Options.ExtraParams)
                        yield return p;
            }
        }

        /// <summary>
        /// Gets Solr parameters for facet queries
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetFacetFieldOptions(QueryOptions options) {
            if (options.Facet == null)
                yield break;
            if (options.Facet.Queries == null || options.Facet.Queries.Count == 0)
                yield break;

            yield return KVP("facet", "true");

            foreach (var fq in options.Facet.Queries)
                foreach (var fqv in facetQuerySerializer.Serialize(fq))
                    yield return fqv;

            if (options.Facet.Prefix != null)
                yield return KVP("facet.prefix", options.Facet.Prefix);
            if (options.Facet.EnumCacheMinDf.HasValue)
                yield return KVP("facet.enum.cache.minDf", options.Facet.EnumCacheMinDf.ToString());
            if (options.Facet.Limit.HasValue)
                yield return KVP("facet.limit", options.Facet.Limit.ToString());
            if (options.Facet.MinCount.HasValue)
                yield return KVP("facet.mincount", options.Facet.MinCount.ToString());
            if (options.Facet.Missing.HasValue)
                yield return KVP("facet.missing", options.Facet.Missing.ToString().ToLowerInvariant());
            if (options.Facet.Offset.HasValue)
                yield return KVP("facet.offset", options.Facet.Offset.ToString());
            if (options.Facet.Sort.HasValue)
                yield return KVP("facet.sort", options.Facet.Sort.ToString().ToLowerInvariant());
        }

        /// <summary>
        /// Gets Solr parameters for defined more-like-this options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetMoreLikeThisParameters(QueryOptions options) {
            if (options.MoreLikeThis == null)
                yield break;
            var mlt = options.MoreLikeThis;
            yield return KVP("mlt", "true");
            if (mlt.Fields != null)
                yield return KVP("mlt.fl", Func.Join(",", mlt.Fields));
            if (mlt.Boost.HasValue)
                yield return KVP("mlt.boost", mlt.Boost.ToString().ToLowerInvariant());
            if (mlt.Count.HasValue)
                yield return KVP("mlt.count", mlt.Count.ToString());
            if (mlt.MaxQueryTerms.HasValue)
                yield return KVP("mlt.maxqt", mlt.MaxQueryTerms.ToString());
            if (mlt.MaxTokens.HasValue)
                yield return KVP("mlt.maxntp", mlt.MaxTokens.ToString());
            if (mlt.MaxWordLength.HasValue)
                yield return KVP("mlt.maxwl", mlt.MaxWordLength.ToString());
            if (mlt.MinDocFreq.HasValue)
                yield return KVP("mlt.mindf", mlt.MinDocFreq.ToString());
            if (mlt.MinTermFreq.HasValue)
                yield return KVP("mlt.mintf", mlt.MinTermFreq.ToString());
            if (mlt.MinWordLength.HasValue)
                yield return KVP("mlt.minwl", mlt.MinWordLength.ToString());
            if (mlt.QueryFields != null && mlt.QueryFields.Count > 0)
                yield return KVP("mlt.qf", Func.Join(",", mlt.QueryFields));
        }

        /// <summary>
        /// Gets Solr parameters for defined filter queries
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetFilterQueries(QueryOptions options) {
            if (options.FilterQueries == null || options.FilterQueries.Count == 0)
                yield break;
            foreach (var fq in options.FilterQueries) {
                yield return new KeyValuePair<string, string>("fq", querySerializer.Serialize(fq));
            }
        }

        /// <summary>
        /// Gets Solr parameters for defined highlightings
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetHighlightingParameters(QueryOptions Options) {
            var param = new Dictionary<string, string>();
            if (Options.Highlight != null) {
                var h = Options.Highlight;
                param["hl"] = "true";
                if (h.Fields != null) {
                    param["hl.fl"] = Func.Join(",", h.Fields);

                    if (h.Snippets.HasValue)
                        param["hl.snippets"] = h.Snippets.Value.ToString();

                    if (h.Fragsize.HasValue)
                        param["hl.fragsize"] = h.Fragsize.Value.ToString();

                    if (h.RequireFieldMatch.HasValue)
                        param["hl.requireFieldMatch"] = h.RequireFieldMatch.Value.ToString().ToLowerInvariant();

                    if (h.AlternateField != null)
                        param["hl.alternateField"] = h.AlternateField;

                    if (h.BeforeTerm != null)
                        param["hl.simple.pre"] = h.BeforeTerm;

                    if (h.AfterTerm != null)
                        param["hl.simple.post"] = h.AfterTerm;

                    if (h.RegexSlop.HasValue)
                        param["hl.regex.slop"] = Convert.ToString(h.RegexSlop.Value, CultureInfo.InvariantCulture);

                    if (h.RegexPattern != null)
                        param["hl.regex.pattern"] = h.RegexPattern;

                    if (h.RegexMaxAnalyzedChars.HasValue)
                        param["hl.regex.maxAnalyzedChars"] = h.RegexMaxAnalyzedChars.Value.ToString();

                    if (h.UsePhraseHighlighter.HasValue)
                        param["hl.usePhraseHighlighter"] = h.UsePhraseHighlighter.Value.ToString().ToLowerInvariant();

                    if (h.UseFastVectorHighlighter.HasValue)
                        param["hl.useFastVectorHighlighter"] = h.UseFastVectorHighlighter.Value.ToString().ToLowerInvariant();

                    if (h.HighlightMultiTerm.HasValue)
                        param["hl.highlightMultiTerm"] = h.HighlightMultiTerm.Value.ToString().ToLowerInvariant();

                    if (h.MergeContiguous.HasValue)
                        param["hl.mergeContiguous"] = h.MergeContiguous.Value.ToString().ToLowerInvariant();

                    if (h.MaxAnalyzedChars.HasValue)
                        param["hl.maxAnalyzedChars"] = h.MaxAnalyzedChars.Value.ToString();

                    if (h.MaxAlternateFieldLength.HasValue)
                        param["hl.maxAlternateFieldLength"] = h.MaxAlternateFieldLength.Value.ToString();

                    if (h.Fragmenter.HasValue)
                        param["hl.fragmenter"] = h.Fragmenter.Value == SolrHighlightFragmenter.Regex ? "regex" : "gap";
                }
            }
            return param;
        }

        /// <summary>
        /// Gets solr parameters for defined spell-checking
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetSpellCheckingParameters(QueryOptions Options) {
            var spellCheck = Options.SpellCheck;
            if (spellCheck != null) {
                yield return KVP("spellcheck", "true");
                if (!string.IsNullOrEmpty(spellCheck.Query))
                    yield return KVP("spellcheck.q", spellCheck.Query);
                if (spellCheck.Build.HasValue)
                    yield return KVP("spellcheck.build", spellCheck.Build.ToString().ToLowerInvariant());
                if (spellCheck.Collate.HasValue)
                    yield return KVP("spellcheck.collate", spellCheck.Collate.ToString().ToLowerInvariant());
                if (spellCheck.Count.HasValue)
                    yield return KVP("spellcheck.count", spellCheck.Count.ToString());
                if (!string.IsNullOrEmpty(spellCheck.Dictionary))
                    yield return KVP("spellcheck.dictionary", spellCheck.Dictionary);
                if (spellCheck.OnlyMorePopular.HasValue)
                    yield return KVP("spellcheck.onlyMorePopular", spellCheck.OnlyMorePopular.ToString().ToLowerInvariant());
                if (spellCheck.Reload.HasValue)
                    yield return KVP("spellcheck.reload", spellCheck.Reload.ToString().ToLowerInvariant());
            }
        }

        /// <summary>
        /// Gets the Solr parameters for stats queries
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetStatsQueryOptions(QueryOptions options) {
            if (options.Stats == null || options.Stats.FieldsWithFacets.Count == 0)
                yield break;

            yield return KVP("stats", "true");

            foreach (var fieldAndFacet in options.Stats.FieldsWithFacets) {
                var field = fieldAndFacet.Key;
                if (string.IsNullOrEmpty(field))
                    continue;
                var facets = fieldAndFacet.Value;
                yield return KVP("stats.field", field);
                if (facets != null && facets.Count > 0) {
                    foreach (var facet in facets) {
                        if (string.IsNullOrEmpty(facet))
                            continue;
                        yield return KVP(string.Format("f.{0}.stats.facet", field), facet);
                    }
                }
            }

            if (options.Stats.Facets == null || options.Stats.Facets.Count == 0)
                yield break;

            foreach (var facet in options.Stats.Facets) {
                if (string.IsNullOrEmpty(facet))
                    continue;
                yield return KVP("stats.facet", facet);
            }
        }

        /// <summary>
        /// Gets the Solr parameters for collapse queries
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetCollapseQueryOptions(QueryOptions options) {
            if (options.Collapse == null || string.IsNullOrEmpty(options.Collapse.Field))
                yield break;

            yield return KVP("collapse.field", options.Collapse.Field);
            if (options.Collapse.Threshold.HasValue)
                yield return KVP("collapse.threshold", options.Collapse.Threshold.ToString());
            yield return KVP("collapse.type", options.Collapse.Type.ToString());
            yield return KVP("collapse.facet", options.Collapse.FacetMode.ToString().ToLowerInvariant());
            if (options.Collapse.MaxDocs.HasValue)
                yield return KVP("collapse.maxdocs", options.Collapse.MaxDocs.ToString());
        }

        /// <summary>
        /// Executes the query and returns results
        /// </summary>
        /// <returns>query results</returns>
        public ISolrQueryResults<T> Execute(ISolrQuery q, QueryOptions options) {
            var param = GetAllParameters(q, options);
            string r = connection.Get("/select", param);
            var qr = resultParser.Parse(r);
            return qr;
        }
    }
}