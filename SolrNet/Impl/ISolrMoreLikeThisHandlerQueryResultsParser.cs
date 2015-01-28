namespace SolrNet.Impl {
    /// <summary>
    /// Query results parser interface
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public interface ISolrMoreLikeThisHandlerQueryResultsParser<T> {
        /// <summary>
        /// Parses solr's mlt handler response
        /// </summary>
        /// <param name="r">solr response</param>
        /// <returns>query results</returns>
        SolrMoreLikeThisHandlerResults<T> Parse(string r);
    }
}