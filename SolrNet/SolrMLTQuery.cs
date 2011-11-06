using System;

namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler query
    /// </summary>
    public abstract class SolrMLTQuery {
        internal SolrMLTQuery() {}

        public T Match<T>(Func<ISolrQuery, T> query, Func<string, T> streamBody, Func<Uri, T> streamUrl) {
            var q = this as SolrMoreLikeThisHandlerQuery;
            var b = this as SolrMoreLikeThisHandlerStreamBodyQuery;
            var u = this as SolrMoreLikeThisHandlerStreamUrlQuery;

            if (q != null)
                return query(q.Query);
            if (b != null)
                return streamBody(b.Body);
            if (u != null)
                return streamUrl(u.Url);
            throw new ArgumentException(string.Format("Unhandled SolrMLTQuery {0}", this));
        }
    }
}
