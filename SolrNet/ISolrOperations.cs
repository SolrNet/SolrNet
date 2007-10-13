using System.Collections.Generic;

namespace SolrNet {
	public interface ISolrOperations<T> where T : ISolrDocument {
		/// <summary>
		/// Commits posts, 
		/// blocking until index changes are flushed to disk and
		/// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
		/// </summary>
		string Commit();

		/// <summary>
		/// Commits posts
		/// </summary>
		/// <param name="waitFlush">block until index changes are flushed to disk</param>
		/// <param name="waitSearcher">block until a new searcher is opened and registered as the main query searcher, making the changes visible.</param>
		string Commit(bool waitFlush, bool waitSearcher);

		string Optimize();
		string Optimize(bool waitFlush, bool waitSearcher);
		string Add(T doc);
		string Add(IEnumerable<T> docs);
		string Delete(T doc);
		string Delete(T doc, bool fromPending, bool fromCommited);
		string Delete(ISolrQuery q);
		string Delete(ISolrQuery q, bool fromPending, bool fromCommited);
		string Delete(string id);
		string Delete(string id, bool fromPending, bool fromCommited);
		ISolrQueryResults<T> Query(ISolrQuery q);
		string Send(ISolrCommand cmd);
	}
}