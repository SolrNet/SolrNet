﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// MoreLikeThisHandler parameters
    /// See http://wiki.apache.org/solr/MoreLikeThisHandler
    /// </summary>
    public class MoreLikeThisHandlerParameters : MoreLikeThisParameters
    {
        /// <summary>
        /// MoreLikeThisHandler parameters
        /// </summary>
        /// <param name="fields">The fields to use for similarity</param>
        public MoreLikeThisHandlerParameters(IEnumerable<string> fields)
            : base(fields) { }

        /// <summary>
        /// Handler to use for MoreLikeThisHandler. Default is /mlt.
        /// </summary>
        public string Handler { get; set; }

        /// <summary>
        /// Should the response include the matched document? If false, the response will look exactly like a normal /select response.
        /// </summary>
        public bool? MatchInclude { get; set; }

        /// <summary>
        /// By default, the MoreLikeThis query operates on the first result for 'q'.
        /// </summary>
        public int? MatchOffset { get; set; }

        public enum InterestingTerms
        {
        	list,
            details,
            none
        }

        /// <summary>
        /// One of: "list", "details", "none" -- this will show what "interesting" terms are used for the MoreLikeThis query. These are the top tf/idf terms.
        /// </summary>
        public InterestingTerms? ShowTerms { get; set; }
    }
}
