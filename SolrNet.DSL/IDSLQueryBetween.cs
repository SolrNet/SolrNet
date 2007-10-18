namespace SolrNet.DSL {
	public interface IDSLQueryBetween<T, RT> where T : ISolrDocument, new() {
		IDSLQuery<T> And(RT i);
	}
}