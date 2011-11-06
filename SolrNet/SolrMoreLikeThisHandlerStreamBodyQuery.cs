namespace SolrNet {
    /// <summary>
    /// MoreLikeThisHandler stream.body query 
    /// </summary>
    public class SolrMoreLikeThisHandlerStreamBodyQuery : ISolrMoreLikeThisHandlerQuery {
        private readonly string _query;

        public SolrMoreLikeThisHandlerStreamBodyQuery(string body) {
            _query = body;
        }

        public string Query {
            get { return _query; }
        }
    }
}