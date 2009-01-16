using System.Collections.Generic;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;

namespace SolrNet {
    public interface ISolrBasicOperations<T>: ISolrBasicReadOnlyOperations<T> {
        void Commit(WaitOptions options);
        void Optimize(WaitOptions options);

        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <returns></returns>
        ISolrBasicOperations<T> Add(IEnumerable<T> docs);

        /// <summary>
        /// Deletes a document (requires the document to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="id">document id to delete</param>
        /// <returns></returns>
        /// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ISolrBasicOperations<T> Delete(string id);

        /// <summary>
        /// Deletes all documents that match a query
        /// </summary>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        ISolrBasicOperations<T> Delete(ISolrQuery q);

        /// <summary>
        /// Sends a custom command
        /// </summary>
        /// <param name="cmd">command to send</param>
        /// <returns>solr response</returns>
        string Send(ISolrCommand cmd);
    }
}