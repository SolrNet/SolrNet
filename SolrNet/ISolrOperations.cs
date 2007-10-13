using System.Collections.Generic;

namespace SolrNet {
	public interface ISolrOperations<T> where T : ISolrDocument {
		/// <summary>
		/// Commits posts, 
		/// blocking until index changes are flushed to disk and
		/// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
		/// </summary>
		void Commit();

		/// <summary>
		/// Commits posts
		/// </summary>
		/// <param name="waitFlush">block until index changes are flushed to disk</param>
		/// <param name="waitSearcher">block until a new searcher is opened and registered as the main query searcher, making the changes visible.</param>
		void Commit(bool waitFlush, bool waitSearcher);

		void Optimize();
		void Optimize(bool waitFlush, bool waitSearcher);
		void Add(T doc);
		void Add(IEnumerable<T> docs);
		void Delete(T doc);
		void Delete(T doc, bool fromPending, bool fromCommited);
		void Delete(ISolrQuery q);
		void Delete(ISolrQuery q, bool fromPending, bool fromCommited);
		void Delete(string id);
		void Delete(string id, bool fromPending, bool fromCommited);
		ISolrQueryResults<T> Query(ISolrQuery q);
		void Send(T cmd);
	}
}