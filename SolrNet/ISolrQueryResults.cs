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

namespace SolrNet {
	/// <summary>
	/// Query results.
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrQueryResults<T> : IAbstractSolrQueryResults<T>  {
        /// <summary>
        /// Max score in these results
        /// </summary>
		double? MaxScore { get; }

        /// <summary>
        /// Highlight results
        /// </summary>
        IDictionary<string, HighlightedSnippets> Highlights { get; set; }

        /// <summary>
        /// Spell checking results
        /// </summary>
	    SpellCheckResults SpellChecking { get; set; }

        /// <summary>
        /// More-like-this results
        /// </summary>
        IDictionary<string, IList<T>> SimilarResults { get; set; }

        /// <summary>
        /// Stats results
        /// </summary>
        IDictionary<string, StatsResult> Stats { get; set; }

        /// <summary>
        /// Collapse results
        /// </summary>
        CollapseResults Collapsing { get; set; }

		/// <summary>
		/// Grouped results
		/// </summary>
		IDictionary<string, GroupedResults<T>> Grouping { get; set; }

		/// <summary>
		/// Pivot faceting results
		/// </summary>
		IDictionary<string, IList<Pivot>> FacetPivots { get; set; }

        /// <summary>
        /// Clustering results
        /// </summary>
        ClusterResults Clusters { get; set; }

        /// <summary>
        /// Term Results
        /// </summary>
        TermsResults Terms { get; set; }
	}
}