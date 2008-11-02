namespace SolrNet.DSL {
	public interface IDSLQueryBetween<T, RT> where T : new() {
		IDSLQuery<T> And(RT i);
	}
}