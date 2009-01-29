#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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
using SolrNet.Utils;

namespace SolrNet {
	public class SolrMultipleCriteriaQuery : ISolrQuery {
		private readonly string q;

		public class Operator {
			public const string OR = "OR";
			public const string AND = "AND";
		}

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery> queries): this(queries, "") {}

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery> queries, string oper) {
			q = Func.Join(string.Format(" {0} ", oper), queries, query => query.Query, true);
            if (!string.IsNullOrEmpty(q))
                q = "(" + q + ")";
		}

        public static SolrMultipleCriteriaQuery Create<T>(IEnumerable<T> queries) where T: ISolrQuery {            
            return new SolrMultipleCriteriaQuery(Func.Cast<ISolrQuery>(queries));
        }

		/// <summary>
		/// query string
		/// </summary>
		public string Query {
			get { return q; }
		}
	}
}