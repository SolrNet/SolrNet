using System;
using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet.Impl {
    public abstract class AbstractSolrQueryResults<T> : List<T> {
        /// <summary>
        /// Total documents found
        /// </summary>
        public int NumFound { get; set; }

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
        public IDictionary<string, DateFacetingResult> FacetDates { get; set; }

        /// <summary>
        /// Facet pivot results
        /// </summary>
        public IDictionary<string, IList<Pivot>> FacetPivots { get; set; }

        protected AbstractSolrQueryResults() {
            FacetQueries = new Dictionary<string, int>();
            FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            FacetDates = new Dictionary<string, DateFacetingResult>();
            FacetPivots = new Dictionary<string, IList<Pivot>>();
        }

        public abstract R Switch<R>(Func<SolrQueryResults<T>, R> queryResults, Func<SolrMoreLikeThisHandlerResults<T>, R> mltResults);

        public void Switch(Action<SolrQueryResults<T>> queryResults, Action<SolrMoreLikeThisHandlerResults<T>> mltResults) {
            Switch(Unit.ActionToFunc(queryResults), Unit.ActionToFunc(mltResults));
        }
    }
}