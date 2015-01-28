namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Selection of top tf/idf terms for <see cref="MoreLikeThisHandlerParameters"/>
    /// </summary>
    public enum InterestingTerms {
        /// <summary>
        /// Simple list of top terms
        /// </summary>
        List,

        /// <summary>
        /// List of top terms and boost used for each term. Unless <see cref="MoreLikeThisParameters.Boost"/> = true all terms will have boost=1.0
        /// </summary>
        Details,

        /// <summary>
        /// No top terms
        /// </summary>
        None
    }
}