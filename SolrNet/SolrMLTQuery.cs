using System;

namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler query
    /// </summary>
    public abstract class SolrMLTQuery {
        internal SolrMLTQuery() {}

        /// <summary>
        /// Uses a web page to look similar documents.
        /// Requires remoteStreaming to be enabled.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static SolrMoreLikeThisHandlerStreamUrlQuery FromStreamUrl(string url) {
            return new SolrMoreLikeThisHandlerStreamUrlQuery(url);
        }

        /// <summary>
        /// Uses text to look for similar documents.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static SolrMoreLikeThisHandlerStreamBodyQuery FromStreamBody(string body) {
            return new SolrMoreLikeThisHandlerStreamBodyQuery(body);
        }

        /// <summary>
        /// Uses the first result in a query to look for similar documents
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static SolrMoreLikeThisHandlerQuery FromQuery(ISolrQuery q) {
            return new SolrMoreLikeThisHandlerQuery(q);
        }

        /// <summary>
        /// Visitor / pattern match
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="streamBody"></param>
        /// <param name="streamUrl"></param>
        /// <returns></returns>
        public abstract T Switch<T>(Func<ISolrQuery, T> query, Func<string, T> streamBody, Func<Uri, T> streamUrl);
    }
}
