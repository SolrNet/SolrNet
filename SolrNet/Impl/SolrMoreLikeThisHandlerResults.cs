using System;
using System.Collections.Generic;

namespace SolrNet.Impl {
    /// <summary>
    /// More-like-this handler results
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SolrMoreLikeThisHandlerResults<T> : AbstractSolrQueryResults<T> {
        /// <summary>
        /// Matched document
        /// </summary>
        public T Match { get; set; }

        /// <summary>
        /// Interesting terms in More-like-this query
        /// </summary>
        public IList<KeyValuePair<string, float>> InterestingTerms { get; set; }

        /// <summary>
        /// More-like-this handler results
        /// </summary>
        public SolrMoreLikeThisHandlerResults() {
            InterestingTerms = new List<KeyValuePair<string, float>>();
        }

        /// <summary>
        /// Visitor / pattern match
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="query"></param>
        /// <param name="moreLikeThis"></param>
        /// <returns></returns>
        public override R Switch<R>(Func<SolrQueryResults<T>, R> query, Func<SolrMoreLikeThisHandlerResults<T>, R> moreLikeThis) {
            return moreLikeThis(this);
        }
    }
}
