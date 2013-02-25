using SolrNet.Utils;
using System;

namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler stream.url query
    /// </summary>
    public class SolrMoreLikeThisHandlerStreamUrlQuery : SolrMLTQuery {
        private readonly Uri url;

        public SolrMoreLikeThisHandlerStreamUrlQuery(string url) {
            this.url = new Uri(UriValidator.ValidateHTTP(url));
        }

        public SolrMoreLikeThisHandlerStreamUrlQuery(Uri url) {
            this.url = url;
        }

        public Uri Url {
            get { return url; }
        }

        public override T Switch<T>(Func<ISolrQuery, T> query, Func<string, T> streamBody, Func<Uri, T> streamUrl) {
            return streamUrl(url);
        }
    }
}