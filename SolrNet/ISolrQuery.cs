namespace SolrNet {
	public interface ISolrQuery<T> {
		string Query { get;}
	}

	public interface ISolrExecutableQuery<T> : ISolrQuery<T> {
		ISolrConnection Connection { get; set;}
		ISolrQueryResults<T> Execute();
	}
}