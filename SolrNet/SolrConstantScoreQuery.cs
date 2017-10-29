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

using System.Globalization;

namespace SolrNet {
    /// <summary>
    /// Applies a constant score to a query or query fragment.
    /// https://lucene.apache.org/solr/guide/6_6/the-standard-query-parser.html#TheStandardQueryParser-ConstantScorewith
    /// </summary>
    public class SolrConstantScoreQuery : AbstractSolrQuery {

        /// <summary>
        /// Applies a constant score to a query or query fragment
        /// </summary>
        /// <param name="query">Query to score</param>
        /// <param name="score">Score</param>
        public SolrConstantScoreQuery(ISolrQuery query, double score) {
            this.Query = query;
            this.Score = score;
        }

        /// <summary>
        /// Constant score
        /// </summary>
        public double Score { get; }

        public ISolrQuery Query { get; }
    }
}