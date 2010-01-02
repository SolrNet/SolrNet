using System;

namespace SolrNet.Commands {
    /// <summary>
    /// Rollbacks all add/deletes made to the index since the last commit.
    /// </summary>
    public class RollbackCommand : ISolrCommand {
        public string Execute(ISolrConnection connection) {
            return connection.Post("/update", "<rollback/>");
        }
    }
}