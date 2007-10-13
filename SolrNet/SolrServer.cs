using System;
using System.Collections.Generic;

namespace SolrNet {
	public class SolrServer<T> : ISolrOperations<T> where T : ISolrDocument {
		private ISolrConnection connection;

		public SolrServer(ISolrConnection connection) {
			this.connection = connection;
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
			return Add(new T[] { doc });
		}

		public string Add(IEnumerable<T> docs) {
			AddCommand<T> cmd = new AddCommand<T>(docs);
			return Send(cmd);
		}

		public string Delete(T doc) {
			throw new NotImplementedException();
		}

		public string Delete(T doc, bool fromPending, bool fromCommited) {
			throw new NotImplementedException();
		}

		public string Delete(ISolrQuery q) {
			throw new NotImplementedException();
		}

		public string Delete(ISolrQuery q, bool fromPending, bool fromCommited) {
			throw new NotImplementedException();
		}

		public string Delete(string id) {
			throw new NotImplementedException();
		}

		public string Delete(string id, bool fromPending, bool fromCommited) {
			throw new NotImplementedException();
		}

		public ISolrQueryResults<T> Query(ISolrQuery q) {
			throw new NotImplementedException();
		}

		public string Send(ISolrCommand cmd) {
			return cmd.Execute(connection);
		}
	}
}