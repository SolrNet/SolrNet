namespace SolrNet.DSL {
	public interface IDSLFacetFieldOptions<T> : IDSLRun<T> where T : new() {
		/// <summary>
		/// Maximum number of constraint counts that should be returned for the facet fields. 
		/// A negative value means unlimited. 
		/// The default value is 100. 
		/// </summary>
		IDSLFacetFieldOptions<T> LimitTo(int limit);

		/// <summary>
		/// Set to true, this parameter indicates that constraints should be sorted by their count. 
		/// If false, facets will be in their natural index order (unicode). 
		/// For terms in the ascii range, this will be alphabetically sorted. 
		/// The default is true if Limit is greater than 0, false otherwise.
		/// </summary>
		IDSLFacetFieldOptions<T> DontSortByCount();

		/// <summary>
		/// Limits the terms on which to facet to those starting with the given string prefix.
		/// </summary>
		IDSLFacetFieldOptions<T> WithPrefix(string prefix);

		/// <summary>
		/// Minimum counts for facet fields that should be included in the response.
		/// The default value is 0.
		/// </summary>
		IDSLFacetFieldOptions<T> WithMinCount(int count);

		/// <summary>
		/// Indicates an offset into the list of constraints to allow paging. 
		/// The default value is 0. 
		/// </summary>
		IDSLFacetFieldOptions<T> StartingAt(int offset);

		/// <summary>
		/// Set to true this param indicates that in addition to the Term based constraints of a facet field, a count of all matching results which have no value for the field should be computed
		/// Default is false
		/// </summary>
		IDSLFacetFieldOptions<T> IncludeMissing();
	}
}