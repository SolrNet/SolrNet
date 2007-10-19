namespace SolrNet.DSL {
	public interface IDSLQuery<T> where T : ISolrDocument, new() {
		IDSLQuery<T> ByRange<RT>(string fieldName, RT from, RT to);
		ISolrQueryResults<T> Run();
		ISolrQueryResults<T> Run(int start, int rows);
		IDSLQueryBy<T> By(string fieldName);
		IDSLQuery<T> ByExample(T doc);
	}
}