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
using System.Linq;

namespace SolrNet {
    /// <summary>
    /// Represents several queries as one
    /// </summary>
    public class SolrMultipleCriteriaQuery : AbstractSolrQuery {
        private readonly IEnumerable<ISolrQuery> queries;
        private readonly string oper;

        /// <summary>
        /// Queries contained in this multiple criteria
        /// </summary>
        public IEnumerable<ISolrQuery> Queries {
            get { return queries; }
        }

        /// <summary>
        /// Operator used for joining these queries
        /// </summary>
        public string Oper {
            get { return oper; }
        }

        /// <summary>
        /// Operator to apply to the included queries
        /// </summary>
		public class Operator {
			public const string OR = "OR";
			public const string AND = "AND";
		}

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery> queries): this(queries, "") {}

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery> queries, string oper) {
		    this.queries = queries;
		    this.oper = oper;
		}

        /// <summary>
        /// Static create helper
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        public static SolrMultipleCriteriaQuery Create(params ISolrQuery[] queries) {
            return Create((IEnumerable<ISolrQuery>) queries);
        }

        /// <summary>
        /// Static create helper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queries"></param>
        /// <returns></returns>
        public static SolrMultipleCriteriaQuery Create<T>(IEnumerable<T> queries) where T: ISolrQuery {
            return new SolrMultipleCriteriaQuery(queries.Cast<ISolrQuery>());
        }
	}
}