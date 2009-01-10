using System.Collections.Generic;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
    /// <summary>
    /// Executable query
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrQueryExecuter<T> : ISolrQueryExecuter<T> where T : new() {
        /// <summary>
        /// Solr response parser, default is XML response parser
        /// </summary>
        public ISolrQueryResultParser<T> ResultParser { get; set; }

        /// <summary>
        /// Connection to use
        /// </summary>
        public ISolrConnection Connection { get; set; }

        public IListRandomizer ListRandomizer { get; set; }

        public IReadOnlyMappingManager MappingManager { get; set; }

        public int DefaultRows { get; set; }

        public SolrQueryExecuter(ISolrConnection connection) {
            Connection = connection;
            ListRandomizer = new ListRandomizer();
            ResultParser = new SolrQueryResultParser<T>();
            MappingManager = ReadOnlyMappingManagerFactory.Create();
            DefaultRows = 100000000;
        }

        public KeyValuePair<T1, T2> KVP<T1, T2>(T1 a, T2 b) {
            return new KeyValuePair<T1, T2>(a, b);
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllParameters(ISolrQuery Query, QueryOptions Options) {
            var param = new List<KeyValuePair<string, string>> {
                KVP("q", Query.Query)
            };
            if (Options != null) {
                if (Options.Start.HasValue)
                    param.Add(KVP("start", Options.Start.ToString()));
                var rows = Options.Rows.HasValue ? Options.Rows.Value : DefaultRows;
                param.Add(KVP("rows", rows.ToString()));
                if (Options.OrderBy != null && Options.OrderBy.Count > 0) {
                    if (Options.OrderBy == SortOrder.Random) {
                        var pk = MappingManager.GetUniqueKey(typeof (T));
                        if (pk.Key == null)
                            throw new NoUniqueKeyException(typeof(T));
                        var executer = new SolrQueryExecuter<T>(Connection) {
                            ListRandomizer = ListRandomizer,
                            ResultParser = ResultParser,
                            MappingManager = MappingManager,
                        };
                        var nr = executer.Execute(Query, new QueryOptions {
                            Fields = new[] {pk.Value},
                        });
                        ListRandomizer.Randomize(nr);
                        var idListQuery = new SolrQueryInList(pk.Value, Func.Select(Func.Take(nr, rows), x => StringHelper.ToNullOrString(pk.Key.GetValue(x, null))));
                        param.RemoveAll(kv => kv.Key == "q");
                        param.Add(KVP("q", idListQuery.Query));
                    } else {
                        param.Add(KVP("sort", Func.Join(",", Options.OrderBy)));
                    }
                }

                if (Options.Fields != null && Options.Fields.Count > 0)
                    param.Add(KVP("fl", Func.Join(",", Options.Fields)));

                if (Options.FacetQueries != null && Options.FacetQueries.Count > 0) {
                    param.Add(KVP("facet", "true"));
                    foreach (var fq in Options.FacetQueries) {
                        foreach (var fqv in fq.Query) {
                            param.Add(fqv);
                        }
                    }
                }

                foreach (var p in GetHighlightingParameters(Options)) {
                    param.Add(p);
                }
            }

            return param;
        }

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
                        param["hl.regex.slop"] = h.RegexSlop.Value.ToString();

                    if (h.RegexPattern != null)
                        param["hl.regex.pattern"] = h.RegexPattern;

                    if (h.RegexMaxAnalyzedChars.HasValue)
                        param["hl.regex.maxAnalyzedChars"] = h.RegexMaxAnalyzedChars.Value.ToString();
                }
            }
            return param;
        }

        /// <summary>
        /// Executes the query and returns results
        /// </summary>
        /// <returns>query results</returns>
        public ISolrQueryResults<T> Execute(ISolrQuery q, QueryOptions options) {
            var param = GetAllParameters(q, options);
            string r = Connection.Get("/select", param);
            var qr = ResultParser.Parse(r);
            return qr;
        }
    }
}