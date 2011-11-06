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
    /// Query results
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrQueryResults<T> : AbstractSolrQueryResults<T> {
        public IDictionary<string, HighlightedSnippets> Highlights { get; set; }
        public SpellCheckResults SpellChecking { get; set; }
        public IDictionary<string, IList<T>> SimilarResults { get; set; }
        public IDictionary<string, StatsResult> Stats { get; set; }
        public CollapseResults Collapsing { get; set; }
        public ClusterResults Clusters { get; set; }
        public TermsResults Terms { get; set; }
        public IDictionary<string, GroupedResults<T>> Grouping { set; get; }

        public SolrQueryResults() {
            SpellChecking = new SpellCheckResults();
            SimilarResults = new Dictionary<string, IList<T>>();
            Stats = new Dictionary<string, StatsResult>();
            Collapsing = new CollapseResults();
            Grouping = new Dictionary<string, GroupedResults<T>>();
            Terms = new TermsResults();
        }
    }
}