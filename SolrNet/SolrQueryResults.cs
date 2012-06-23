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

using System;
using System.Collections.Generic;
using SolrNet.Impl;

namespace SolrNet {
    /// <summary>
    /// Query results
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrQueryResults<T> : AbstractSolrQueryResults<T> {
        /// <summary>
        /// Highlight results
        /// </summary>
        public IDictionary<string, HighlightedSnippets> Highlights { get; set; }

        /// <summary>
        /// Spellchecking results
        /// </summary>
        public SpellCheckResults SpellChecking { get; set; }

        /// <summary>
        /// More-like-this component results
        /// </summary>
        public IDictionary<string, IList<T>> SimilarResults { get; set; }

        /// <summary>
        /// Stats component results
        /// </summary>
        public IDictionary<string, StatsResult> Stats { get; set; }

        /// <summary>
        /// Collapse results
        /// </summary>
        [Obsolete("Use result grouping instead")]
        public CollapseResults Collapsing { get; set; }

        /// <summary>
        /// Clustering results
        /// </summary>
        public ClusterResults Clusters { get; set; }

        /// <summary>
        /// TermsComponent results
        /// </summary>
        public TermsResults Terms { get; set; }

		/// <summary>
		/// TermVectorComponent results
		/// </summary>
        public ICollection<TermVectorDocumentResult> TermVectorResults { get; set; }

        /// <summary>
        /// Grouping results
        /// </summary>
        public IDictionary<string, GroupedResults<T>> Grouping { set; get; }

        public SolrQueryResults() {
            SpellChecking = new SpellCheckResults();
            SimilarResults = new Dictionary<string, IList<T>>();
            Stats = new Dictionary<string, StatsResult>();
            Collapsing = new CollapseResults();
            Grouping = new Dictionary<string, GroupedResults<T>>();
            Terms = new TermsResults();
        }

        public override R Switch<R>(Func<SolrQueryResults<T>, R> query, Func<SolrMoreLikeThisHandlerResults<T>, R> moreLikeThis) {
            return query(this);
        }
    }
}