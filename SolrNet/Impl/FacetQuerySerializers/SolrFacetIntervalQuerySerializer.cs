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
    /// Serializes <see cref="SolrFacetIntervalQuery"/>
    /// </summary>
    public class SolrFacetIntervalQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetIntervalQuery>
    {

        private static readonly Regex localParamsRx = new Regex(@"\{![^\}]+\}", RegexOptions.Compiled);

        private readonly ISolrFieldSerializer fieldSerializer;

        /// <summary>
        /// Serializes <see cref="SolrFacetIntervalQuery"/>
        /// </summary>
        public SolrFacetIntervalQuerySerializer(ISolrFieldSerializer fieldSerializer)
        {
            this.fieldSerializer = fieldSerializer;
        }

        public string SerializeSingle(object o)
        {
            return fieldSerializer.Serialize(o).First().FieldValue;
        }

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetIntervalQuery q)
        {
            var fieldWithoutLocalParams = localParamsRx.Replace(q.Field, "");
            yield return KV.Create("facet.interval", q.Field);
            if (q.Sets != null && q.Sets.Count > 0)
                foreach (var interval in q.Sets)
                    yield return KV.Create(string.Format("f.{0}.facet.interval.set", fieldWithoutLocalParams), interval.ToString());
        }
    }
}
