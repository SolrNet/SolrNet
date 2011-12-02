using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// MoreLikeThisHandler parameters
    /// See http://wiki.apache.org/solr/MoreLikeThisHandler
    /// </summary>
    public class MoreLikeThisHandlerParameters : MoreLikeThisParameters {
        /// <summary>
        /// MoreLikeThisHandler parameters
        /// </summary>
        /// <param name="fields">The fields to use for similarity</param>
        public MoreLikeThisHandlerParameters(IEnumerable<string> fields)
            : base(fields) {}

        /// <summary>
        /// Should the response include the matched document? If false, the response will look exactly like a normal /select response.
        /// </summary>
        public bool? MatchInclude { get; set; }

        /// <summary>
        /// By default, the MoreLikeThis query operates on the first result for 'q'.
        /// </summary>
        public int? MatchOffset { get; set; }

        /// <summary>
        /// One of: "list", "details", "none" -- this will show what "interesting" terms are used for the MoreLikeThis query. These are the top tf/idf terms.
        /// </summary>
        public InterestingTerms? ShowTerms { get; set; }
    }
}