namespace SolrNet {
	public interface ISolrDocumentSerializer<T> where T : ISolrDocument {
		string Serialize(T doc);
	}
}