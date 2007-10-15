using SolrNet.Tests;

namespace SolrNet.DSL {
	public class DeleteBy {
		private ISolrConnection connection;

		public DeleteBy(ISolrConnection connection) {
			this.connection = connection;
		}

		public void ById(string id) {
			DeleteCommand cmd = new DeleteCommand();
			cmd.DeleteParam = new DeleteByIdParam(id);
			cmd.Execute(connection);
		}

		public void ByQuery<T>(ISolrQuery<T> q) where T : ISolrDocument {
			DeleteCommand cmd = new DeleteCommand();
			cmd.DeleteParam = new DeleteByQueryParam<T>(q);
			cmd.Execute(connection);
		}
	}
}