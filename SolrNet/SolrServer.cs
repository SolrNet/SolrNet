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
		public void Commit() {
			new CommitCommand().Execute(connection);
		}

		/// <summary>
		/// Commits posts
		/// </summary>
		/// <param name="waitFlush">block until index changes are flushed to disk</param>
		/// <param name="waitSearcher">block until a new searcher is opened and registered as the main query searcher, making the changes visible.</param>
		public void Commit(bool waitFlush, bool waitSearcher) {
			CommitCommand cmd = new CommitCommand();
			cmd.WaitFlush = waitFlush;
			cmd.WaitSearcher = waitSearcher;
			cmd.Execute(connection);
		}

		public void Optimize() {
			new OptimizeCommand().Execute(connection);
		}

		public void Optimize(bool waitFlush, bool waitSearcher) {
			OptimizeCommand optimize = new OptimizeCommand();
			optimize.WaitFlush = waitFlush;
			optimize.WaitSearcher = waitSearcher;
			optimize.Execute(connection);
		}

		public void Add(T doc) {
			throw new NotImplementedException();
		}

		public void Add(IEnumerable<T> docs) {
			throw new NotImplementedException();
		}

		public void Delete(T doc) {
			throw new NotImplementedException();
		}

		public void Delete(T doc, bool fromPending, bool fromCommited) {
			throw new NotImplementedException();
		}

		public void Delete(ISolrQuery q) {
			throw new NotImplementedException();
		}

		public void Delete(ISolrQuery q, bool fromPending, bool fromCommited) {
			throw new NotImplementedException();
		}

		public void Delete(string id) {
			throw new NotImplementedException();
		}

		public void Delete(string id, bool fromPending, bool fromCommited) {
			throw new NotImplementedException();
		}

		public ISolrQueryResults<T> Query(ISolrQuery q) {
			throw new NotImplementedException();
		}

		public void Send(T cmd) {
			throw new NotImplementedException();
		}
	}
}