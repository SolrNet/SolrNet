namespace SolrNet.DSL {
	public interface IDSLQueryRange<T> : IDSLQuery<T> where T : ISolrDocument, new() {
		IDSLQuery<T> Exclusive();
		IDSLQuery<T> Inclusive();
	}
}