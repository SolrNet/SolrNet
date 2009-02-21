namespace SolrNet.DSL {
    public static class Query {
        public static ISolrQuery Simple(string s) {
            return new SolrQuery(s);
        }

        public static FieldDefinition Field(string field) {
            return new FieldDefinition(field);
        }
    }
}