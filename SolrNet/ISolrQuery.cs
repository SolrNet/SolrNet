namespace SolrNet {
	public interface ISolrQuery<T> where T : ISolrDocument {
		string Query { get; }
	}
}