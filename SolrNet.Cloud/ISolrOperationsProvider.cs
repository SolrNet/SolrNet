namespace SolrNet.Cloud
{
    /// <summary>
    /// Solr operations interface
    /// </summary>
    public interface ISolrOperationsProvider {
        /// <summary>
        /// Gets basic operations
        /// <param name="isPostConnection">True if it's need to use solt post connection</param>
        /// </summary>
        ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool isPostConnection = false);

        /// <summary>
        /// Gets operations
        /// <param name="isPostConnection">True if it's need to use solt post connection</param>
        /// </summary>
        ISolrOperations<T> GetOperations<T>(string url, bool isPostConnection = false);
    }
}