using System;
using System.Collections.Generic;
using SolrNet.Impl.FieldSerializers;

namespace SolrNet {
    public class SolrFacetDateQuery : ISolrFacetQuery {
        private readonly string field;
        private readonly DateTime start;
        private readonly DateTime end;
        private readonly string gap;
        private readonly DateTimeFieldSerializer dateSerializer = new DateTimeFieldSerializer();
        private readonly BoolFieldSerializer boolSerializer = new BoolFieldSerializer();

        public SolrFacetDateQuery(string field, DateTime start, DateTime end, string gap) {
            this.field = field;
            this.start = start;
            this.end = end;
            this.gap = gap;
            Other = new List<FacetDateOther>();
        }

        public bool? HardEnd { get; set; }

        public ICollection<FacetDateOther> Other { get; set; }

        public KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

        public IEnumerable<KeyValuePair<string, string>> Query {
            get {
                yield return KV("facet.date", field);
                yield return KV(string.Format("facet.{0}.start", field), dateSerializer.SerializeDate(start));
                yield return KV(string.Format("facet.{0}.end", field), dateSerializer.SerializeDate(end));
                yield return KV(string.Format("facet.{0}.gap", field), gap);
                if (HardEnd.HasValue)
                    yield return KV(string.Format("facet.{0}.hardend", field), boolSerializer.SerializeBool(HardEnd.Value));
                if (Other != null && Other.Count > 0)
                    foreach (var o in Other)
                        yield return KV(string.Format("facet.{0}.other", field), o.ToString());
            }
        }
    }
}