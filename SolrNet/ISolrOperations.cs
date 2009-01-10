using System.Collections.Generic;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;

namespace SolrNet {
	/// <summary>
	/// Consolidating interface, exposes all operations
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrOperations<T> : ISolrReadOnlyOperations<T> {
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
        void Commit(WaitOptions options);

        void Optimize();
        void Optimize(bool waitFlush, bool waitSearcher);
        void Optimize(WaitOptions options);

		/// <summary>
		/// Adds / updates a document
		/// </summary>
		/// <param name="doc">document to add/update</param>
		/// <returns></returns>
        ISolrOperations<T> Add(T doc);

		/// <summary>
		/// Adds / updates several documents at once
		/// </summary>
		/// <param name="docs">documents to add/update</param>
		/// <returns></returns>
        ISolrOperations<T> Add(IEnumerable<T> docs);

		/// <summary>
		/// Deletes a document (requires the document to have a unique key defined with non-null value)
		/// </summary>
		/// <param name="doc">document to delete</param>
		/// <returns></returns>
		/// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ISolrOperations<T> Delete(T doc);

		/// <summary>
		/// Deletes a document (requires the document to have a unique key defined)
		/// </summary>
		/// <param name="doc">document to delete</param>
		/// <param name="fromPending">deletes document from pending (not committed) documents</param>
		/// <param name="fromCommited">deletes document from committed documents</param>
		/// <returns></returns>
		/// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ISolrOperations<T> Delete(T doc, bool fromPending, bool fromCommited);

		/// <summary>
		/// Deletes all documents that match a query
		/// </summary>
		/// <param name="q">query to match</param>
		/// <returns></returns>
        ISolrOperations<T> Delete(ISolrQuery q);

		/// <summary>
		/// Deletes all documents that match a query
		/// </summary>
		/// <param name="q">query to match</param>
		/// <param name="fromPending">deletes document from pending (not committed) documents</param>
		/// <param name="fromCommited">deletes document from committed documents</param>
		/// <returns></returns>
        ISolrOperations<T> Delete(ISolrQuery q, bool fromPending, bool fromCommited);

		/// <summary>
		/// Deletes a document by its id (unique key)
		/// </summary>
		/// <param name="id">document key</param>
		/// <returns></returns>
        ISolrOperations<T> Delete(string id);

		/// <summary>
		/// Deletes a document by its id (unique key)
		/// </summary>
		/// <param name="id">document key</param>
		/// <param name="fromPending">deletes document from pending (not committed) documents</param>
		/// <param name="fromCommited">deletes document from committed documents</param>
		/// <returns></returns>
        ISolrOperations<T> Delete(string id, bool fromPending, bool fromCommited);

	    /// <summary>
		/// Sends a custom command
		/// </summary>
		/// <param name="cmd">command to send</param>
		/// <returns>solr response</returns>
		string Send(ISolrCommand cmd);
	}
}