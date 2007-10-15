namespace SolrNet {
	public interface ISolrExecutableQuery<T> : ISolrQuery<T> where T : ISolrDocument {
		ISolrConnection Connection { get; set; }
		ISolrQueryResults<T> Execute();
	}
}