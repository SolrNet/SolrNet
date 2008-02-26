using SolrNet.Commands;
using SolrNet.Commands.Parameters;

namespace SolrNet.DSL {
	public class DeleteBy {
		private readonly ISolrConnection connection;

		public DeleteBy(ISolrConnection connection) {
			this.connection = connection;
		}

		public void ById(string id) {
			var cmd = new DeleteCommand(new DeleteByIdParam(id));
			cmd.Execute(connection);
		}

		public void ByQuery(ISolrQuery q) {
			var cmd = new DeleteCommand(new DeleteByQueryParam(q));
			cmd.Execute(connection);
		}

		public void ByQuery(string q) {
			ByQuery(new SolrQuery(q));
		}
	}
}