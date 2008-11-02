namespace SolrNet.DSL {
	public interface IDSLQueryRange<T> : IDSLQuery<T> where T : new() {
		IDSLQuery<T> Exclusive();
		IDSLQuery<T> Inclusive();
	}
}