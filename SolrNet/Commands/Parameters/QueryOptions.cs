#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using SolrNet.Impl;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Query options
    /// </summary>
	public partial class QueryOptions: CommonQueryOptions {

		/// <summary>
		/// Sort order.
		/// By default, it's "score desc"
		/// </summary>
		public ICollection<SortOrder> OrderBy { get; set; }

		/// <summary>
		/// Highlighting parameters
		/// </summary>
		public HighlightingParameters Highlight { get; set; }

        /// <summary>
        /// Spell-checking parameters
        /// </summary>
        public SpellCheckingParameters SpellCheck { get; set; }

        /// <summary>
        /// Terms parameters
        /// </summary>
        public TermsParameters Terms { get; set; }

        /// <summary>
        /// More-like-this parameters
        /// </summary>
        public MoreLikeThisParameters MoreLikeThis { get; set; }

        /// <summary>
        /// This parameter can be used to return the stats for a specific query on top of the results that are normally returned.  Included in the stats are
        /// min, max, sum, count, missing, sumOfSquares, mean, and stddev values.  
        /// </summary>
        public StatsParameters Stats { get; set; }

        /// <summary>
        /// This parameter can be used to collapse - or group - documents by the unique values of a specified field. Included in the results are the number of
        /// records by document key and by field value
        /// </summary>
        public CollapseParameters Collapse { get; set; }

        /// <summary>
        /// The collapsing query parser and the expand component combine to form an approach to grouping documents for field collapsing in search results.
        /// The expand component requires Solr 4.8+
        /// </summary>
        /// <see href="https://cwiki.apache.org/confluence/display/solr/Collapse+and+Expand+Results"/>
        public CollapseExpandParameters CollapseExpand { get; set; }

		/// <summary>
		/// This parameter can be used to collapse - or group - documents by the unique values of a specified field. Included in the results are the number of
		/// records by document key and by field value
		/// </summary>
		public TermVectorParameters TermVector { get; set; }

		/// <summary>
		/// (only SOLR 4.0)
		/// This parameter can be used to collapse - or group - documents by the unique values of a specified field. Included in the results are the number of
		/// records by document key and by field value
		/// </summary>
		public GroupingParameters Grouping { get; set; }

        /// <summary>
        /// This parameter can be used to group query results into clusters based on document similarity 
        /// </summary>
        public ClusteringParameters Clustering { get; set; }

        /// <summary>
        /// Request handler parameters
        /// </summary>
        public RequestHandlerParameters RequestHandler { get; set; }

        public QueryOptions() {
			OrderBy = new List<SortOrder>();
		}
	}
}
