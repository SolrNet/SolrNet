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
using System.Text.RegularExpressions;
using SolrNet.Utils;

namespace SolrNet.Impl.FacetQuerySerializers {
    public class SolrFacetFieldQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetFieldQuery> {
        private static readonly Regex localParamsRx = new Regex(@"\{![^\}]+\}", RegexOptions.Compiled);

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetFieldQuery q) {
            yield return KV.Create("facet.field", q.Field);
            var fieldWithoutLocalParams = localParamsRx.Replace(q.Field, "");
            if (q.Prefix != null)
                yield return KV.Create(string.Format("f.{0}.facet.prefix", fieldWithoutLocalParams), q.Prefix);
            if (q.Sort.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.sort", fieldWithoutLocalParams), q.Sort.ToString().ToLowerInvariant());
            if (q.Limit.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.limit", fieldWithoutLocalParams), q.Limit.ToString());
            if (q.Offset.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.offset", fieldWithoutLocalParams), q.Offset.ToString());
            if (q.MinCount.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.mincount", fieldWithoutLocalParams), q.MinCount.ToString());
            if (q.Missing.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.missing", fieldWithoutLocalParams), q.Missing.ToString().ToLowerInvariant());
            if (q.EnumCacheMinDf.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.enum.cache.minDf", fieldWithoutLocalParams), q.EnumCacheMinDf.ToString());
            if (!string.IsNullOrEmpty(q.Contains))
                yield return KV.Create(string.Format("f.{0}.facet.contains", fieldWithoutLocalParams), q.Contains);
            if (!string.IsNullOrEmpty(q.Contains) && q.ContainsIgnoreCase.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.contains.ignoreCase", fieldWithoutLocalParams), q.ContainsIgnoreCase.ToString().ToLowerInvariant());
            if (q.Exists.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.exists", fieldWithoutLocalParams), q.Exists.ToString().ToLowerInvariant());
        }
    }
}
