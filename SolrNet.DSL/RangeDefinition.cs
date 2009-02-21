namespace SolrNet.DSL {
    public class RangeDefinition<T> {
        private readonly string fieldName;
        private readonly T from;

        public RangeDefinition(string fieldName, T from) {
            this.fieldName = fieldName;
            this.from = from;
        }

        public ISolrQuery To(T to) {
            return new SolrQueryByRange<T>(fieldName, from, to);
        }
    }
}