using System.Collections.Generic;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
    public class SolrServer<T> : ISolrOperations<T> where T : new() {
        private readonly ISolrConnection connection;
        public IReadOnlyMappingManager MappingManager { get; set; }

        /// <summary>
        /// Solr response parser, default is XML response parser
        /// </summary>
        public ISolrQueryResultParser<T> ResultParser { get; set; }

        public ISolrQueryExecuter<T> QueryExecuter { get; set; }

        private SolrServer() {
            MappingManager = ReadOnlyMappingManagerFactory.Create();
            ResultParser = new SolrQueryResultParser<T>();
        }

        public SolrServer(string serverURL) : this() {
            connection = new SolrConnection(serverURL);
            QueryExecuter = new SolrQueryExecuter<T>(connection);
        }

        public SolrServer(ISolrConnection connection) : this() {
            this.connection = connection;
            QueryExecuter = new SolrQueryExecuter<T>(connection);
        }

        /// <summary>
        /// Commits posts, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        public void Commit() {
            Send(new CommitCommand());
        }

        /// <summary>
        /// Commits posts
        /// </summary>
        /// <param name="waitFlush">block until index changes are flushed to disk</param>
        /// <param name="waitSearcher">block until a new searcher is opened and registered as the main query searcher, making the changes visible.</param>
        public void Commit(bool waitFlush, bool waitSearcher) {
            var cmd = new CommitCommand {WaitFlush = waitFlush, WaitSearcher = waitSearcher};
            Send(cmd);
        }

        public void Commit(WaitOptions options) {
            var cmd = new CommitCommand {WaitFlush = options.WaitFlush, WaitSearcher = options.WaitSearcher};
            Send(cmd);
        }

        public void Optimize() {
            Send(new OptimizeCommand());
        }

        public void Optimize(bool waitFlush, bool waitSearcher) {
            var optimize = new OptimizeCommand {WaitFlush = waitFlush, WaitSearcher = waitSearcher};
            Send(optimize);
        }

        public void Optimize(WaitOptions options) {
            var cmd = new OptimizeCommand {WaitFlush = options.WaitFlush, WaitSearcher = options.WaitSearcher};
            Send(cmd);
        }

        public ISolrOperations<T> Add(T doc) {
            Add(new[] {doc});
            return this;
        }

        public ISolrOperations<T> Add(IEnumerable<T> docs) {
            var cmd = new AddCommand<T>(docs);
            Send(cmd);
            return this;
        }

        public ISolrOperations<T> Delete(T doc) {
            var id = GetId(doc);
            Delete(id.ToString());
            return this;
        }

        private object GetId(T doc) {
            var prop = MappingManager.GetUniqueKey(typeof (T)).Key;
            var id = prop.GetValue(doc, null);
            if (id == null)
                throw new NoUniqueKeyException(typeof (T));
            return id;
        }

        public ISolrOperations<T> Delete(T doc, bool fromPending, bool fromCommited) {
            var id = GetId(doc);
            Delete(id.ToString(), fromPending, fromCommited);
            return this;
        }

        public ISolrOperations<T> Delete(ISolrQuery q) {
            var delete = new DeleteCommand(new DeleteByQueryParam(q));
            delete.Execute(connection);
            return this;
        }

        public ISolrOperations<T> Delete(ISolrQuery q, bool fromPending, bool fromCommited) {
            var delete = new DeleteCommand(new DeleteByQueryParam(q)) {FromCommitted = fromCommited, FromPending = fromPending};
            delete.Execute(connection);
            return this;
        }

        public ISolrOperations<T> Delete(string id) {
            var delete = new DeleteCommand(new DeleteByIdParam(id));
            delete.Execute(connection);
            return this;
        }

        public ISolrOperations<T> Delete(string id, bool fromPending, bool fromCommited) {
            var delete = new DeleteCommand(new DeleteByIdParam(id)) {FromCommitted = fromCommited, FromPending = fromPending};
            delete.Execute(connection);
            return this;
        }

        public ISolrQueryResults<T> Query(string q) {
            return Query(new SolrQuery(q));
        }

        public ISolrQueryResults<T> Query(string q, ICollection<SortOrder> orders) {
            return Query(new SolrQuery(q), orders);
        }

        public ISolrQueryResults<T> Query(string q, QueryOptions options) {
            return QueryExecuter.Execute(new SolrQuery(q), options);
        }

        public ISolrQueryResults<T> Query(ISolrQuery query) {
            return Query(query, new QueryOptions());
        }

        public ISolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders) {
            return Query(query, new QueryOptions {OrderBy = orders});
        }

        public ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return Query(query.Query, options);
        }

        public string Send(ISolrCommand cmd) {
            return cmd.Execute(connection);
        }

        public void Ping() {
            new PingCommand().Execute(connection);
        }
    }
}