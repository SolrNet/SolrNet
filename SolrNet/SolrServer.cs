using System;
using System.Collections.Generic;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;

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
		    MappingManager = new AttributesMappingManager();
		    ResultParser = new SolrQueryResultParser<T>();
		}

	    public SolrServer(string serverURL): this() {
			connection = new SolrConnection(serverURL);
            QueryExecuter = new SolrQueryExecuter<T>(connection);
        }

		public SolrServer(ISolrConnection connection): this() {
			this.connection = connection;
            QueryExecuter = new SolrQueryExecuter<T>(connection);
		}

	    /// <summary>
		/// Commits posts, 
		/// blocking until index changes are flushed to disk and
		/// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
		/// </summary>
		public string Commit() {
			return Send(new CommitCommand());
		}

		/// <summary>
		/// Commits posts
		/// </summary>
		/// <param name="waitFlush">block until index changes are flushed to disk</param>
		/// <param name="waitSearcher">block until a new searcher is opened and registered as the main query searcher, making the changes visible.</param>
		public string Commit(bool waitFlush, bool waitSearcher) {
			var cmd = new CommitCommand {WaitFlush = waitFlush, WaitSearcher = waitSearcher};
			return Send(cmd);
		}

		public string Commit(WaitOptions options) {
			var cmd = new CommitCommand {WaitFlush = options.WaitFlush, WaitSearcher = options.WaitSearcher};
			return Send(cmd);
		}

		public string Optimize() {
			return Send(new OptimizeCommand());
		}

		public string Optimize(bool waitFlush, bool waitSearcher) {
			var optimize = new OptimizeCommand {WaitFlush = waitFlush, WaitSearcher = waitSearcher};
			return Send(optimize);
		}

		public string Optimize(WaitOptions options) {
			var cmd = new OptimizeCommand {WaitFlush = options.WaitFlush, WaitSearcher = options.WaitSearcher};
			return Send(cmd);
		}

		public string Add(T doc) {
			return Add(new[] {doc});
		}

		public string Add(IEnumerable<T> docs) {
			var cmd = new AddCommand<T>(docs);
			return Send(cmd);
		}

		public string Delete(T doc) {
			var id = GetId(doc);
			return Delete(id.ToString());
		}

		private object GetId(T doc) {
			var prop = MappingManager.GetUniqueKey(typeof (T)).Key;
			if (prop == null)
				throw new NoUniqueKeyException();
			var id = prop.GetValue(doc, null);
			if (id == null)
				throw new NoUniqueKeyException();
			return id;
		}

		public string Delete(T doc, bool fromPending, bool fromCommited) {
			var id = GetId(doc);
			return Delete(id.ToString(), fromPending, fromCommited);
		}

		public string Delete(ISolrQuery q) {
			var delete = new DeleteCommand(new DeleteByQueryParam(q));
			return delete.Execute(connection);
		}

		public string Delete(ISolrQuery q, bool fromPending, bool fromCommited) {
			var delete = new DeleteCommand(new DeleteByQueryParam(q)) {FromCommitted = fromCommited, FromPending = fromPending};
			return delete.Execute(connection);
		}

		public string Delete(string id) {
			var delete = new DeleteCommand(new DeleteByIdParam(id));
			return delete.Execute(connection);
		}

		public string Delete(string id, bool fromPending, bool fromCommited) {
			var delete = new DeleteCommand(new DeleteByIdParam(id)) {FromCommitted = fromCommited, FromPending = fromPending};
			return delete.Execute(connection);
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

		public string Ping() {
			return new PingCommand().Execute(connection);
		}
	}
}