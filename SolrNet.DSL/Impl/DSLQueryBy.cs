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
    public class DSLQueryBy<T> : IDSLQueryBy<T> {
        private readonly string fieldName;
        private readonly ISolrConnection connection;
        private readonly ISolrQuery query;

        public DSLQueryBy(string fieldName, ISolrConnection connection, ISolrQuery query) {
            this.fieldName = fieldName;
            this.connection = connection;
            this.query = query;
        }

        public IDSLQuery<T> Is(string s) {
            return new DSLQuery<T>(connection,
                                   new SolrMultipleCriteriaQuery(new[] {
                                       query,
                                       new SolrQueryByField(fieldName, s)
                                   }));
        }

        public IDSLQueryBetween<T, RT> Between<RT>(RT i) {
            return new DSLQueryBetween<T, RT>(fieldName, connection, query, i);
        }
    }
}