using System.Collections.Generic;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;

namespace SolrNet {
    public class SolrBasicServer<T> : ISolrBasicOperations<T> where T : new() {
        private readonly ISolrConnection connection;

        public ISolrQueryExecuter<T> QueryExecuter { get; set; }

        public SolrBasicServer(string serverURL) {
            connection = new SolrConnection(serverURL);
            QueryExecuter = new SolrQueryExecuter<T>(connection);
        }

        public SolrBasicServer(ISolrConnection connection) {
            this.connection = connection;
            QueryExecuter = new SolrQueryExecuter<T>(connection);
        }

        public void Commit(WaitOptions options) {
            options = options ?? new WaitOptions();
            var cmd = new CommitCommand {WaitFlush = options.WaitFlush, WaitSearcher = options.WaitSearcher};
            Send(cmd);
        }

        public void Optimize(WaitOptions options) {
            options = options ?? new WaitOptions();
            var cmd = new OptimizeCommand {WaitFlush = options.WaitFlush, WaitSearcher = options.WaitSearcher};
            Send(cmd);
        }

        public ISolrBasicOperations<T> Add(IEnumerable<T> docs) {
            var cmd = new AddCommand<T>(docs);
            Send(cmd);
            return this;
        }

        public ISolrBasicOperations<T> Delete(string id) {
            var cmd = new DeleteCommand(new DeleteByIdParam(id));
            Send(cmd);
            return this;
        }

        public ISolrBasicOperations<T> Delete(ISolrQuery q) {
            var delete = new DeleteCommand(new DeleteByQueryParam(q));
            delete.Execute(connection);
            return this;
        }

        public ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return QueryExecuter.Execute(query, options);
        }

        public string Send(ISolrCommand cmd) {
            return cmd.Execute(connection);
        }

        public void Ping() {
            new PingCommand().Execute(connection);
        }
    }
}