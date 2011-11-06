namespace SolrNet {
    /// <summary>
    /// Standard MoreLikeThisHandlerQuery
    /// </summary>
    public class SolrMoreLikeThisHandlerQuery : SolrMLTQuery {
        private readonly ISolrQuery query;

        public SolrMoreLikeThisHandlerQuery(ISolrQuery query) {
            this.query = query;
        }

        public ISolrQuery Query {
            get { return query; }
        }
    }
}