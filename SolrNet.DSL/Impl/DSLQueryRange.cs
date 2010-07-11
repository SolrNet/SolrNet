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

namespace SolrNet.DSL.Impl {
    public class DSLQueryRange<T, RT> : DSLQuery<T>, IDSLQueryRange<T> {
        private readonly string fieldName;
        private readonly RT from;
        private readonly RT to;
        private readonly ISolrQuery prevQuery;

        public DSLQueryRange(ISolrConnection connection, ISolrQuery query, string fieldName, RT from, RT to) : base(connection) {
            this.query = new SolrMultipleCriteriaQuery(new[] {
                query,
                new SolrQueryByRange<RT>(fieldName, from, to)
            });
            prevQuery = query;
            this.fieldName = fieldName;
            this.from = from;
            this.to = to;
        }

        private ISolrQuery buildFinalQuery(bool inclusive) {
            return new SolrMultipleCriteriaQuery(new[] {
                prevQuery,
                new SolrQueryByRange<RT>(fieldName, from, to, inclusive)
            });
        }

        public IDSLQuery<T> Exclusive() {
            return new DSLQuery<T>(connection, buildFinalQuery(false));
        }

        public IDSLQuery<T> Inclusive() {
            return new DSLQuery<T>(connection, buildFinalQuery(true));
        }
    }
}