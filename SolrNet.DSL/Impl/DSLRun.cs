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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.DSL.Impl {
    public class DSLRun<T> : IDSLRun<T> {
        protected readonly ICollection<SortOrder> order = new List<SortOrder>();
        protected readonly ICollection<ISolrFacetQuery> facets = new List<ISolrFacetQuery>();
        protected readonly HighlightingParameters highlight;
        protected ISolrConnection connection;
        protected ISolrQuery query;

        public DSLRun(ISolrConnection connection) {
            this.connection = connection;
        }

        public DSLRun(ISolrConnection connection, ISolrQuery query) : this(connection) {
            this.query = query;
        }

        public DSLRun(ISolrConnection connection, ISolrQuery query, ICollection<SortOrder> order) : this(connection, query) {
            this.order = order;
        }

        public DSLRun(ISolrConnection connection, ISolrQuery query, ICollection<SortOrder> order, ICollection<ISolrFacetQuery> facets, HighlightingParameters highlight) : this(connection, query, order) {
            this.facets = facets;
            this.highlight = highlight;
        }

        private ISolrQueryExecuter<T> NewQueryExecuter() {
            return new SolrQueryExecuter<T>(
                ServiceLocator.Current.GetInstance<ISolrQueryResultParser<T>>(),
                connection,
                ServiceLocator.Current.GetInstance<ISolrQuerySerializer>(),
                ServiceLocator.Current.GetInstance<ISolrFacetQuerySerializer>());
        }

        public ISolrQueryResults<T> Run() {
            var exe = NewQueryExecuter();
            return exe.Execute(query, new QueryOptions {
                OrderBy = order,
                Facet = new FacetParameters {
                    Queries = facets,
                },
                Highlight = highlight,
            });
        }

        public ISolrQueryResults<T> Run(int start, int rows) {
            var exe = NewQueryExecuter(); 
            return exe.Execute(query, new QueryOptions {
                OrderBy = order,
                Facet = new FacetParameters {
                    Queries = facets,
                },
                Start = start,
                Rows = rows,
                Highlight = highlight,
            });
        }

        public IDSLRun<T> OrderBy(string fieldName) {
            var newOrder = new List<SortOrder>(order) {new SortOrder(fieldName)};
            return new DSLRun<T>(connection, query, newOrder);
        }

        public IDSLRun<T> OrderBy(string fieldName, Order o) {
            var newOrder = new List<SortOrder>(order) {new SortOrder(fieldName, o)};
            return new DSLRun<T>(connection, query, newOrder);
        }

        public IDSLFacetFieldOptions<T> WithFacetField(string fieldName) {
            var facetFieldQuery = new SolrFacetFieldQuery(fieldName);
            var newFacets = new List<ISolrFacetQuery>(facets) {facetFieldQuery};
            return new DSLFacetFieldOptions<T>(connection, query, order, newFacets, facetFieldQuery);
        }

        public IDSLRun<T> WithFacetQuery(string q) {
            return WithFacetQuery(new SolrQuery(q));
        }

        public IDSLRun<T> WithFacetQuery(ISolrQuery q) {
            var newFacets = new List<ISolrFacetQuery>(facets) {new SolrFacetQuery(q)};
            return new DSLRun<T>(connection, query, order, newFacets, highlight);
        }

        public IDSLRun<T> WithHighlighting(HighlightingParameters parameters) {
            return new DSLRun<T>(connection, query, order, facets, parameters);
        }

        public IDSLRun<T> WithHighlightingFields(params string[] fields) {
            return WithHighlighting(new HighlightingParameters {
                Fields = fields,
            });
        }

        public override string ToString() {
            var l = new List<string>();
            var serializer = ServiceLocator.Current.GetInstance<ISolrQuerySerializer>();
            if (query != null)
                l.Add(serializer.Serialize(query));
            if (highlight != null)
                l.Add("highlight=" + highlight);
            if (facets != null)
                l.Add("facets=" + string.Join("\n", facets.Select(f => f.ToString()).ToArray()));

            return string.Join("\n", l.ToArray());
        }
    }
}