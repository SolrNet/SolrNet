namespace SolrNet {
    /// <summary>
    /// Standard MoreLikeThisHandlerQuery
    /// </summary>
    public class SolrMoreLikeThisHandlerQuery : ISolrMoreLikeThisHandlerQuery {
        private readonly string _query;

        public SolrMoreLikeThisHandlerQuery(string query) {
            _query = query;
        }

        public string Query {
            get { return _query; }
        }
    }
}