#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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
using System.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl.FacetQuerySerializers {
    public class SolrFacetDateQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetDateQuery> {

        private readonly ISolrFieldSerializer fieldSerializer;

        public SolrFacetDateQuerySerializer(ISolrFieldSerializer fieldSerializer) {
            this.fieldSerializer = fieldSerializer;
        }

        private static KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

        public string SerializeSingle(object o) {
            return fieldSerializer.Serialize(o).First().FieldValue;
        }

        public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetDateQuery q) {
            yield return KV("facet.date", q.Field);
            yield return KV(string.Format("f.{0}.facet.date.start", q.Field), SerializeSingle(q.Start));
            yield return KV(string.Format("f.{0}.facet.date.end", q.Field), SerializeSingle(q.End));
            yield return KV(string.Format("f.{0}.facet.date.gap", q.Field), q.Gap);
            if (q.HardEnd.HasValue)
                yield return KV(string.Format("f.{0}.facet.date.hardend", q.Field), SerializeSingle(q.HardEnd.Value));
            if (q.Other != null && q.Other.Count > 0)
                foreach (var o in q.Other)
                    yield return KV(string.Format("f.{0}.facet.date.other", q.Field), o.ToString());
        }
    }
}