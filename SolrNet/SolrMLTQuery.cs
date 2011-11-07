using System;

namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler query
    /// </summary>
    public abstract class SolrMLTQuery {
        internal SolrMLTQuery() {}

        public abstract T Match<T>(Func<ISolrQuery, T> query, Func<string, T> streamBody, Func<Uri, T> streamUrl);
    }
}
