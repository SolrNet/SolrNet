using System;
using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet.Impl {
    public abstract class AbstractSolrQueryResults<T> : List<T> {
        /// <summary>
        /// CursorMark token returned for deep pagination.
        /// Only present if explicitly requested through <see cref="StartOrCursor"/>
        /// </summary>
        public StartOrCursor.Cursor NextCursorMark { get; set; }

        /// <summary>
        /// Total documents found
        /// </summary>
        public long NumFound { get; set; }

        /// <summary>
        /// Start of the results
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// Max score in these results
        /// </summary>
        public double? MaxScore { get; set; }

        /// <summary>
        /// Query response header
        /// </summary>
        public ResponseHeader Header { get; set; }

        /// <summary>
        /// Facet query results
        /// </summary>
        public IDictionary<string, int> FacetQueries { get; set; }

        /// <summary>
        /// Facet field results
        /// </summary>
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> FacetFields { get; set; }

        /// <summary>
        /// Facet date results
        /// </summary>
        [Obsolete("As of Solr 3.1 has been deprecated, as of Solr 6.6 unsupported.")]
        public IDictionary<string, DateFacetingResult> FacetDates { get; set; }

        /// <summary>
        /// Facet ranges results
        /// </summary>
        public IDictionary<string, RangeFacetingResult> FacetRanges { get; set; }

        /// <summary>
        /// Facet ranges results
        /// </summary>
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> FacetIntervals { get; set; }

        /// <summary>
        /// Facet pivot results
        /// </summary>
        public IDictionary<string, IList<Pivot>> FacetPivots { get; set; }

        protected AbstractSolrQueryResults() {
            FacetQueries = new Dictionary<string, int>();
            FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            FacetDates = new Dictionary<string, DateFacetingResult>();
            FacetRanges = new Dictionary<string, RangeFacetingResult>();
            FacetIntervals = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            FacetPivots = new Dictionary<string, IList<Pivot>>();
        }

        /// <summary>
        /// Visitor / pattern match
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="query"></param>
        /// <param name="moreLikeThis"></param>
        /// <returns></returns>
        public abstract R Switch<R>(Func<SolrQueryResults<T>, R> query, Func<SolrMoreLikeThisHandlerResults<T>, R> moreLikeThis);

        /// <summary>
        /// Visitor / pattern match
        /// </summary>
        /// <param name="query"></param>
        /// <param name="moreLikeThis"></param>
        public void Switch(Action<SolrQueryResults<T>> query, Action<SolrMoreLikeThisHandlerResults<T>> moreLikeThis) {
            Switch(F.ToFunc(query), F.ToFunc(moreLikeThis));
        }
    }
}
