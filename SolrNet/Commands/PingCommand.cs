using System.Collections.Generic;

namespace SolrNet.Commands {
	public class PingCommand : ISolrCommand {
		public string Execute(ISolrConnection connection) {
			return connection.Get("/admin/ping", new Dictionary<string, string>());
		}
	}
}