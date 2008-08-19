using System.Collections.Generic;

namespace SolrNet.DSL {
	public class DSLFacetFieldOptions<T> : DSLRun<T>, IDSLFacetFieldOptions<T> where T : ISolrDocument, new() {
		private readonly SolrFacetFieldQuery facetQuery;

		public DSLFacetFieldOptions(ISolrConnection connection, ISolrQuery query, ICollection<SortOrder> order, ICollection<ISolrFacetQuery> facets, SolrFacetFieldQuery facetQuery) : base(connection, query, order, facets) {
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