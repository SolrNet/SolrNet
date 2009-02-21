using System;
using SolrNet.Utils;

namespace SolrNet.DSL {
    public class FieldDefinition {
        private readonly string fieldName;

        public FieldDefinition(string fieldName) {
            this.fieldName = fieldName;
        }

        public RangeDefinition<T> From<T>(T from) {
            return new RangeDefinition<T>(fieldName, from);
        }

        public ISolrQuery In<T>(params T[] values) {
            return new SolrQueryInList(fieldName, Func.Select(values, v => Convert.ToString((object) v)));
        }

        public ISolrQuery Is<T>(T value) {
            return new SolrQueryByField(fieldName, Convert.ToString(value));
        }

        public ISolrQuery HasAnyValue() {
            return From("*").To("*");
        }
    }
}