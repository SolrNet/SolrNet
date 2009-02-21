namespace SolrNet {
    /// <summary>
    /// Queries documents that have any value in the specified field
    /// </summary>
    public class SolrHasValueQuery : AbstractSolrQuery {
        private readonly string field;

        public SolrHasValueQuery(string field) {
            this.field = field;
        }


        public override string Query {
            get {
                var range = new SolrQueryByRange<string>(field, "*", "*");
                return range.Query;
            }
        }
    }
}