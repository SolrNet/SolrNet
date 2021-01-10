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

using System.Text;

namespace SolrNet.Impl.QuerySerializers {
    public class MultipleCriteriaQuerySerializer : SingleTypeQuerySerializer<SolrMultipleCriteriaQuery> {
        private readonly ISolrQuerySerializer serializer;

        public MultipleCriteriaQuerySerializer(ISolrQuerySerializer serializer) {
            this.serializer = serializer;
        }

        /// <inheritdoc />
        public override string Serialize(SolrMultipleCriteriaQuery q) {
            var queryBuilder = new StringBuilder();
            foreach (var query in q.Queries) {
                if (query == null)
                    continue;
                var sq = serializer.Serialize(query);
                if (string.IsNullOrEmpty(sq))
                    continue;
                if (queryBuilder.Length > 0)
                    queryBuilder.AppendFormat(" {0} ", q.Oper);
                queryBuilder.Append(sq);
            }
            var queryString = queryBuilder.ToString();
            if (!string.IsNullOrEmpty(queryString))
                queryString = "(" + queryString + ")";
            return queryString;
        }
    }
}
