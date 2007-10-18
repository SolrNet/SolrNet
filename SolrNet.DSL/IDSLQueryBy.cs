namespace SolrNet.DSL {
	public interface IDSLQueryBy<T> where T : ISolrDocument, new() {
		IDSLQuery<T> Is(string s);
		IDSLQueryBetween<T, RT> Between<RT>(RT i);
	}
}