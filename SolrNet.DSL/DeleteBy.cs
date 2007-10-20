using SolrNet.Tests;

namespace SolrNet.DSL {
	public class DeleteBy {
		private ISolrConnection connection;

		public DeleteBy(ISolrConnection connection) {
			this.connection = connection;
		}

		public void ById(string id) {
			DeleteCommand cmd = new DeleteCommand(new DeleteByIdParam(id));
			cmd.Execute(connection);
		}

		public void ByQuery<T>(ISolrQuery<T> q) where T : ISolrDocument {
			DeleteCommand cmd = new DeleteCommand(new DeleteByQueryParam<T>(q));
			cmd.Execute(connection);
		}

		public void ByQuery<T>(string q) where T : ISolrDocument {
			ByQuery(new SolrQuery<T>(q));
		}
	}
}