using System.Collections.Generic;
using SolrNet.Exceptions;

namespace SolrNet {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
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
		/// <summary>
		/// Adds / updates a document
		/// </summary>
		/// <param name="doc">document to add/update</param>
		/// <returns></returns>
		string Add(T doc);

		/// <summary>
		/// Adds / updates several documents at once
		/// </summary>
		/// <param name="docs">documents to add/update</param>
		/// <returns></returns>
		string Add(IEnumerable<T> docs);

		/// <summary>
		/// Deletes a document (requires the document to have a unique key defined with non-null value)
		/// </summary>
		/// <param name="doc">document to delete</param>
		/// <returns></returns>
		/// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
		string Delete(T doc);

		/// <summary>
		/// Deletes a document (requires the document to have a unique key defined)
		/// </summary>
		/// <param name="doc">document to delete</param>
		/// <param name="fromPending">deletes document from pending (not committed) documents</param>
		/// <param name="fromCommited">deletes document from committed documents</param>
		/// <returns></returns>
		/// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
		string Delete(T doc, bool fromPending, bool fromCommited);

		/// <summary>
		/// Deletes all documents that match a query
		/// </summary>
		/// <param name="q">query to match</param>
		/// <returns></returns>
		string Delete(ISolrQuery<T> q);

		/// <summary>
		/// Deletes all documents that match a query
		/// </summary>
		/// <param name="q">query to match</param>
		/// <param name="fromPending">deletes document from pending (not committed) documents</param>
		/// <param name="fromCommited">deletes document from committed documents</param>
		/// <returns></returns>
		string Delete(ISolrQuery<T> q, bool fromPending, bool fromCommited);

		/// <summary>
		/// Deletes a document by its id (unique key)
		/// </summary>
		/// <param name="id">document key</param>
		/// <returns></returns>
		string Delete(string id);

		/// <summary>
		/// Deletes a document by its id (unique key)
		/// </summary>
		/// <param name="id">document key</param>
		/// <param name="fromPending">deletes document from pending (not committed) documents</param>
		/// <param name="fromCommited">deletes document from committed documents</param>
		/// <returns></returns>
		string Delete(string id, bool fromPending, bool fromCommited);

		/// <summary>
		/// Queries documents
		/// </summary>
		/// <param name="q">query to execute</param>
		/// <returns>query results</returns>
		ISolrQueryResults<T> Query(ISolrQuery<T> q);

		/// <summary>
		/// Sends a custom command
		/// </summary>
		/// <param name="cmd">command to send</param>
		/// <returns>solr response</returns>
		string Send(ISolrCommand cmd);
	}
}