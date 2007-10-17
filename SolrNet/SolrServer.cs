using System.Collections.Generic;
using System.Reflection;
using SolrNet.Exceptions;
using SolrNet.Tests;

namespace SolrNet {
	public class SolrServer<T> : ISolrOperations<T> where T : ISolrDocument, new() {
		private ISolrConnection connection;
		private IUniqueKeyFinder<T> uniqueKeyFinder = new UniqueKeyFinder<T>();

		public SolrServer(ISolrConnection connection) {
			this.connection = connection;
		}

		public IUniqueKeyFinder<T> UniqueKeyFinder {
			get { return uniqueKeyFinder; }
			set { uniqueKeyFinder = value; }
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
			DeleteCommand delete = new DeleteCommand();
			delete.DeleteParam = new DeleteByQueryParam<T>(q);
			return delete.Execute(connection);
		}

		public string Delete(ISolrQuery<T> q, bool fromPending, bool fromCommited) {
			DeleteCommand delete = new DeleteCommand();
			delete.DeleteParam = new DeleteByQueryParam<T>(q);
			delete.FromCommitted = fromCommited;
			delete.FromPending = fromPending;
			return delete.Execute(connection);
		}

		public string Delete(string id) {
			DeleteCommand delete = new DeleteCommand();
			delete.DeleteParam = new DeleteByIdParam(id);
			return delete.Execute(connection);
		}

		public string Delete(string id, bool fromPending, bool fromCommited) {
			DeleteCommand delete = new DeleteCommand();
			delete.DeleteParam = new DeleteByIdParam(id);
			delete.FromCommitted = fromCommited;
			delete.FromPending = fromPending;
			return delete.Execute(connection);
		}

		public ISolrQueryResults<T> Query(ISolrQuery<T> q) {
			ISolrExecutableQuery<T> exe = new SolrExecutableQuery<T>(connection, q.Query);
			return exe.Execute();
		}

		public string Send(ISolrCommand cmd) {
			return cmd.Execute(connection);
		}
	}
}