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
using System.Text.RegularExpressions;
using SolrNet.Utils;

namespace SolrNet.Impl.FacetQuerySerializers {
    /// <summary>
    /// Serializes <see cref="SolrFacetDateQuery"/>
    /// </summary>
    [Obsolete("As of Solr 3.1 has been deprecated, as of Solr 6.6 unsupported.")]
    public class SolrFacetDateQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetDateQuery> {

        private static readonly Regex localParamsRx = new Regex(@"\{![^\}]+\}", RegexOptions.Compiled);

        private readonly ISolrFieldSerializer fieldSerializer;

        /// <summary>
        /// Serializes <see cref="SolrFacetDateQuery"/>
        /// </summary>
        public SolrFacetDateQuerySerializer(ISolrFieldSerializer fieldSerializer) {
            this.fieldSerializer = fieldSerializer;
        }

        public string SerializeSingle(object o) {
            return fieldSerializer.Serialize(o).First().FieldValue;
        }

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetDateQuery q) {
            var fieldWithoutLocalParams = localParamsRx.Replace(q.Field, ""); 
            yield return KV.Create("facet.date", q.Field);
            yield return KV.Create(string.Format("f.{0}.facet.date.start", fieldWithoutLocalParams), SerializeSingle(q.Start));
            yield return KV.Create(string.Format("f.{0}.facet.date.end", fieldWithoutLocalParams), SerializeSingle(q.End));
            yield return KV.Create(string.Format("f.{0}.facet.date.gap", fieldWithoutLocalParams), q.Gap);
            if (q.HardEnd.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.date.hardend", fieldWithoutLocalParams), SerializeSingle(q.HardEnd.Value));
            if (q.Other != null && q.Other.Count > 0)
                foreach (var o in q.Other)
                    yield return KV.Create(string.Format("f.{0}.facet.date.other", fieldWithoutLocalParams), o.ToString());
            if (q.Include != null && q.Include.Count > 0)
                foreach (var i in q.Include)
                    yield return KV.Create(string.Format("f.{0}.facet.date.include", fieldWithoutLocalParams), i.ToString());
        }
    }
}
