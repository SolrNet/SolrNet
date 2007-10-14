namespace SolrNet {
	public interface ISolrQueryResultParser<T> where T : ISolrDocument {
		ISolrQueryResults<T> Parse(string r);
	}
}