namespace SolrNet {
	public interface ISolrCommand {
		string Execute(ISolrConnection connection);
	}
}