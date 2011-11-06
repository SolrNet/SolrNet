namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler stream.body query 
    /// </summary>
    public class SolrMoreLikeThisHandlerStreamBodyQuery : SolrMLTQuery {
        private readonly string body;

        public SolrMoreLikeThisHandlerStreamBodyQuery(string body) {
            this.body = body;
        }

        public string Body {
            get { return body; }
        }
    }
}