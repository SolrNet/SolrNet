using System;
using SolrNet.Utils;

namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler stream.url query
    /// </summary>
    public class SolrMoreLikeThisHandlerStreamUrlQuery : ISolrMoreLikeThisHandlerQuery {
        private readonly string _query;

        public SolrMoreLikeThisHandlerStreamUrlQuery(string url) {
            _query = UriValidator.ValidateHTTP(url);
        }

        public SolrMoreLikeThisHandlerStreamUrlQuery(Uri url) {
            _query = url.AbsoluteUri;
        }

        public string Query {
            get { return _query; }
        }
    }
}