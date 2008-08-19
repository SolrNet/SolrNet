using System.Collections.Generic;
using SolrNet.Commands.Parameters;

namespace SolrNet.DSL {
	public class DSLRun<T> : IDSLRun<T> where T : ISolrDocument, new() {
		protected readonly ICollection<SortOrder> order = new List<SortOrder>();
		protected readonly ICollection<ISolrFacetQuery> facets = new List<ISolrFacetQuery>();
		protected ISolrConnection connection;
		protected ISolrQuery query;

		public DSLRun(ISolrConnection connection) {
			this.connection = connection;
		}

		public DSLRun(ISolrConnection connection, ISolrQuery query): this(connection) {
			this.query = query;
		}

		public DSLRun(ISolrConnection connection, ISolrQuery query, ICollection<SortOrder> order): this(connection, query) {
			this.order = order;
		}

		public DSLRun(ISolrConnection connection, ISolrQuery query, ICollection<SortOrder> order, ICollection<ISolrFacetQuery> facets): this(connection, query, order) {
			this.facets = facets;
		}

		public ISolrQueryResults<T> Run() {
			var exe = new SolrQueryExecuter<T>(connection, query) {
				Options = new QueryOptions {
					OrderBy = order,
					FacetQueries = facets,
				},
			};
			return exe.Execute();
		}

		public ISolrQueryResults<T> Run(int start, int rows) {
			var exe = new SolrQueryExecuter<T>(connection, query) {
				Options = new QueryOptions {
					OrderBy = order,
					FacetQueries = facets,
					Start = start, 
					Rows = rows
				}
			};
			return exe.Execute();
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
			var newFacets = new List<ISolrFacetQuery>(facets) { new SolrFacetQuery(q) };
			return new DSLRun<T>(connection, query, order, newFacets);
		}
	}
}