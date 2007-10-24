using System.Collections.Generic;
using System.Reflection;
using SolrNet.Exceptions;
using SolrNet.Tests;

namespace SolrNet {
	public class SolrServer<T> : ISolrOperations<T> where T : ISolrDocument, new() {
		private ISolrConnection connection;
		private IUniqueKeyFinder<T> uniqueKeyFinder = new UniqueKeyFinder<T>();
		private ISolrQueryResultParser<T> resultParser = new SolrQueryResultParser<T>();

		public SolrServer(string serverURL) {
			connection = new SolrConnection(serverURL);
		}

		public SolrServer(ISolrConnection connection) {
			this.connection = connection;
		}

		public IUniqueKeyFinder<T> UniqueKeyFinder {
			get { return uniqueKeyFinder; }
			set { uniqueKeyFinder = value; }
		}

		/// <summary>
		/// Solr response parser, default is XML response parser
		/// </summary>
		public ISolrQueryResultParser<T> ResultParser {
			get { return resultParser; }
			set { resultParser = value; }
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
			CommitCommand cmd = new CommitCommand();
			cmd.WaitFlush = waitFlush;
			cmd.WaitSearcher = waitSearcher;
			return Send(cmd);
		}

		public string Optimize() {
			return Send(new OptimizeCommand());
		}

		public string Optimize(bool waitFlush, bool waitSearcher) {
			OptimizeCommand optimize = new OptimizeCommand();
			optimize.WaitFlush = waitFlush;
			optimize.WaitSearcher = waitSearcher;
			return Send(optimize);
		}

		public string Add(T doc) {
			return Add(new T[] {doc});
		}

		public string Add(IEnumerable<T> docs) {
			AddCommand<T> cmd = new AddCommand<T>(docs);
			return Send(cmd);
		}

		public string Delete(T doc) {
			object id = GetId(doc);
			return Delete(id.ToString());
		}

		private object GetId(T doc) {
			PropertyInfo prop = uniqueKeyFinder.UniqueKeyProperty;
			if (prop == null)
				throw new NoUniqueKeyException();
			object id = prop.GetValue(doc, null);
			if (id == null)
				throw new NoUniqueKeyException();
			return id;
		}

		public string Delete(T doc, bool fromPending, bool fromCommited) {
			object id = GetId(doc);
			return Delete(id.ToString(), fromPending, fromCommited);
		}

		public string Delete(ISolrQuery<T> q) {
			DeleteCommand delete = new DeleteCommand(new DeleteByQueryParam<T>(q));
			return delete.Execute(connection);
		}

		public string Delete(ISolrQuery<T> q, bool fromPending, bool fromCommited) {
			DeleteCommand delete = new DeleteCommand(new DeleteByQueryParam<T>(q));
			delete.FromCommitted = fromCommited;
			delete.FromPending = fromPending;
			return delete.Execute(connection);
		}

		public string Delete(string id) {
			DeleteCommand delete = new DeleteCommand(new DeleteByIdParam(id));
			return delete.Execute(connection);
		}

		public string Delete(string id, bool fromPending, bool fromCommited) {
			DeleteCommand delete = new DeleteCommand(new DeleteByIdParam(id));
			delete.FromCommitted = fromCommited;
			delete.FromPending = fromPending;
			return delete.Execute(connection);
		}

		public ISolrQueryResults<T> Query(ISolrQuery<T> query) {
			return Query(query, null);
		}

		public ISolrQueryResults<T> Query(ISolrQuery<T> query, int start, int rows) {
			return Query(query, start, rows, null);
		}

		public ISolrQueryResults<T> Query(ISolrQuery<T> query, int start, int rows, ICollection<SortOrder> orders) {
			SolrQueryExecuter<T> exe = new SolrQueryExecuter<T>(connection, query);
			exe.ResultParser = resultParser;
			exe.OrderBy = orders;
			return exe.Execute(start, rows);
		}

		public ISolrQueryResults<T> Query(ISolrQuery<T> query, ICollection<SortOrder> orders) {
			SolrQueryExecuter<T> exe = new SolrQueryExecuter<T>(connection, query);
			exe.ResultParser = resultParser;
			exe.OrderBy = orders;
			return exe.Execute();
		}

		public string Send(ISolrCommand cmd) {
			return cmd.Execute(connection);
		}
	}
}