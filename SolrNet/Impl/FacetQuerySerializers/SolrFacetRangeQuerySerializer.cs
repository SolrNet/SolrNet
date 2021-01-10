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

namespace SolrNet.Impl.FacetQuerySerializers
{
    /// <summary>
    /// Serializes <see cref="SolrFacetRangeQuery"/>
    /// </summary>
    public class SolrFacetRangeQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetRangeQuery>
    {

        private static readonly Regex localParamsRx = new Regex(@"\{![^\}]+\}", RegexOptions.Compiled);

        private readonly ISolrFieldSerializer fieldSerializer;

        /// <summary>
        /// Serializes <see cref="SolrFacetRangeQuery"/>
        /// </summary>
        public SolrFacetRangeQuerySerializer(ISolrFieldSerializer fieldSerializer)
        {
            this.fieldSerializer = fieldSerializer;
        }

        public string SerializeSingle(object o)
        {
            return fieldSerializer.Serialize(o).First().FieldValue;
        }

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetRangeQuery q)
        {
            var fieldWithoutLocalParams = localParamsRx.Replace(q.Field, "");
            yield return KV.Create("facet.range", q.Field);
            yield return KV.Create(string.Format("f.{0}.facet.range.start", fieldWithoutLocalParams), SerializeSingle(q.Start));
            yield return KV.Create(string.Format("f.{0}.facet.range.end", fieldWithoutLocalParams), SerializeSingle(q.End));
            yield return KV.Create(string.Format("f.{0}.facet.range.gap", fieldWithoutLocalParams), q.Gap);
            if (q.HardEnd.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.range.hardend", fieldWithoutLocalParams), SerializeSingle(q.HardEnd.Value));
            if (q.Other != null && q.Other.Count > 0)
                foreach (var o in q.Other)
                    yield return KV.Create(string.Format("f.{0}.facet.range.other", fieldWithoutLocalParams), o.ToString());
            if (q.Include != null && q.Include.Count > 0)
                foreach (var i in q.Include)
                    yield return KV.Create(string.Format("f.{0}.facet.range.include", fieldWithoutLocalParams), i.ToString());
            if (q.Method != null)
                yield return KV.Create($"f.{fieldWithoutLocalParams}.facet.range.method", SerializeSingle(q.Method));
        }
    }
}
