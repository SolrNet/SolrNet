using System;

namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler query
    /// </summary>
    public abstract class SolrMLTQuery {
        internal SolrMLTQuery() {}

        public static SolrMoreLikeThisHandlerStreamUrlQuery StreamUrl(string url) {
            return new SolrMoreLikeThisHandlerStreamUrlQuery(url);
        }

        public static SolrMoreLikeThisHandlerStreamBodyQuery StreamBody(string body) {
            return new SolrMoreLikeThisHandlerStreamBodyQuery(body);
        }

        public static SolrMoreLikeThisHandlerQuery Query(ISolrQuery q) {
            return new SolrMoreLikeThisHandlerQuery(q);
        }

        public abstract T Match<T>(Func<ISolrQuery, T> query, Func<string, T> streamBody, Func<Uri, T> streamUrl);
    }
}
