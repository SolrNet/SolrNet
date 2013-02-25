using System;

namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler stream.body query 
    /// </summary>
    public class SolrMoreLikeThisHandlerStreamBodyQuery : SolrMLTQuery {
        private readonly string body;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        public SolrMoreLikeThisHandlerStreamBodyQuery(string body) {
            this.body = body;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Body {
            get { return body; }
        }

        public override T Switch<T>(Func<ISolrQuery, T> query, Func<string, T> streamBody, Func<Uri, T> streamUrl) {
            return streamBody(body);
        }
    }
}