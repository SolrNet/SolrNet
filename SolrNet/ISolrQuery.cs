namespace SolrNet {
	public interface ISolrQuery<T> where T : ISolrDocument {
		string Query { get; }
	}

	public interface ISolrExecutableQuery<T> : ISolrQuery<T> where T : ISolrDocument {
		ISolrConnection Connection { get; set; }
		ISolrQueryResults<T> Execute();
	}
}