namespace SolrNet.DSL {
	public interface IDSLRun<T> where T : ISolrDocument, new() {
		ISolrQueryResults<T> Run();
		ISolrQueryResults<T> Run(int start, int rows);
		IDSLRun<T> OrderBy(string fieldName);
		IDSLRun<T> OrderBy(string fieldName, Order o);
		IDSLRun<T> WithFacetField(string fieldName);
		IDSLRun<T> WithFacetQuery(string query);
		IDSLRun<T> WithFacetQuery(ISolrQuery query);
	}
}