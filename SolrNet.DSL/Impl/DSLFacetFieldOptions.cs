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

using System.Collections.Generic;

namespace SolrNet.DSL.Impl {
    public class DSLFacetFieldOptions<T> : DSLRun<T>, IDSLFacetFieldOptions<T> {
        private readonly SolrFacetFieldQuery facetQuery;

        public DSLFacetFieldOptions(ISolrConnection connection, ISolrQuery query, ICollection<SortOrder> order, ICollection<ISolrFacetQuery> facets, SolrFacetFieldQuery facetQuery) 
            : base(connection, query, order, facets, null) {
            this.facetQuery = facetQuery;
        }

        public IDSLFacetFieldOptions<T> LimitTo(int limit) {
            facetQuery.Limit = limit;
            return new DSLFacetFieldOptions<T>(connection, query, order, facets, facetQuery);
        }

        public IDSLFacetFieldOptions<T> DontSortByCount() {
            facetQuery.Sort = false;
            return new DSLFacetFieldOptions<T>(connection, query, order, facets, facetQuery);
        }

        public IDSLFacetFieldOptions<T> WithPrefix(string prefix) {
            facetQuery.Prefix = prefix;
            return new DSLFacetFieldOptions<T>(connection, query, order, facets, facetQuery);
        }

        public IDSLFacetFieldOptions<T> WithMinCount(int count) {
            facetQuery.MinCount = count;
            return new DSLFacetFieldOptions<T>(connection, query, order, facets, facetQuery);
        }

        public IDSLFacetFieldOptions<T> StartingAt(int offset) {
            facetQuery.Offset = offset; 
            return new DSLFacetFieldOptions<T>(connection, query, order, facets, facetQuery);
        }

        public IDSLFacetFieldOptions<T> IncludeMissing() {
            facetQuery.Missing = true;
            return new DSLFacetFieldOptions<T>(connection, query, order, facets, facetQuery);
        }
    }
}