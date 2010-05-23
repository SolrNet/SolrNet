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

namespace SolrNet.Impl.FacetQuerySerializers {
    public class SolrFacetFieldQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetFieldQuery> {
        private static KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

        public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetFieldQuery q) {
            yield return KV("facet.field", q.Field);
            if (q.Prefix != null)
                yield return KV(string.Format("f.{0}.facet.prefix", q.Field), q.Prefix);
            if (q.Sort.HasValue)
                yield return KV(string.Format("f.{0}.facet.sort", q.Field), q.Sort.ToString().ToLowerInvariant());
            if (q.Limit.HasValue)
                yield return KV(string.Format("f.{0}.facet.limit", q.Field), q.Limit.ToString());
            if (q.Offset.HasValue)
                yield return KV(string.Format("f.{0}.facet.offset", q.Field), q.Offset.ToString());
            if (q.MinCount.HasValue)
                yield return KV(string.Format("f.{0}.facet.mincount", q.Field), q.MinCount.ToString());
            if (q.Missing.HasValue)
                yield return KV(string.Format("f.{0}.facet.missing", q.Field), q.Missing.ToString().ToLowerInvariant());
            if (q.EnumCacheMinDf.HasValue)
                yield return KV(string.Format("f.{0}.facet.enum.cache.minDf", q.Field), q.EnumCacheMinDf.ToString());
        }
    }
}