namespace SolrNet.DSL {
	public interface IDSLQuery<T> : IDSLRun<T> where T : new() {
		IDSLQueryRange<T> ByRange<RT>(string fieldName, RT from, RT to);
		IDSLQueryBy<T> By(string fieldName);
		IDSLQuery<T> ByExample(T doc);
	}
}