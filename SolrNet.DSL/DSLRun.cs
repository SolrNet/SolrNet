using System.Collections.Generic;
using SolrNet.Commands.Parameters;
using SolrNet.Utils;

namespace SolrNet.DSL {
	public class DSLRun<T> : IDSLRun<T> where T : new() {
		protected readonly ICollection<SortOrder> order = new List<SortOrder>();
		protected readonly ICollection<ISolrFacetQuery> facets = new List<ISolrFacetQuery>();
		protected readonly HighlightingParameters highlight;
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

		public DSLRun(ISolrConnection connection, ISolrQuery query, ICollection<SortOrder> order, ICollection<ISolrFacetQuery> facets, HighlightingParameters highlight): this(connection, query, order) {
			this.facets = facets;
			this.highlight = highlight;
		}

		public ISolrQueryResults<T> Run() {
			var exe = new SolrQueryExecuter<T>(connection, query) {
				Options = new QueryOptions {
					OrderBy = order,
					FacetQueries = facets,
					Highlight = highlight,
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
					Rows = rows,
					Highlight = highlight,
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
			if (query != null)
				l.Add(query.Query);
			if (highlight != null)
				l.Add("highlight=" + highlight);
			if (facets != null)
				l.Add("facets=" + string.Join("\n", Func.ToArray(Func.Select(facets, f => f.ToString()))));

			return string.Join("\n", l.ToArray());
		}
	}
}