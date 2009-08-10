#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
                yield return KV(string.Format("f.{0}.facet.date.start", field), dateSerializer.SerializeDate(start));
                yield return KV(string.Format("f.{0}.facet.date.end", field), dateSerializer.SerializeDate(end));
                yield return KV(string.Format("f.{0}.facet.date.gap", field), gap);
                if (HardEnd.HasValue)
                    yield return KV(string.Format("f.{0}.facet.date.hardend", field), boolSerializer.SerializeBool(HardEnd.Value));
                if (Other != null && Other.Count > 0)
                    foreach (var o in Other)
                        yield return KV(string.Format("f.{0}.facet.date.other", field), o.ToString());
            }
        }
    }
}