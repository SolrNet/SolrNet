namespace SolrNet {
	public interface ISolrDocumentSerializer<T> {
		string Serialize(T doc);
	}
}