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
    }
}
