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

namespace SolrNet {
	/// <summary>
	/// Basic solr query
	/// </summary>	
	public class SolrQuery : ISolrQuery {
		private readonly string query;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="query">solr query to execute</param>
		public SolrQuery(string query) {
			this.query = query;
		}

		/// <summary>
		/// query to execute
		/// </summary>
		public string Query {
			get { return query; }
		}

		static SolrQuery() {
			All = new SolrQuery("*:*");
		}

        /// <summary>
        /// Represents a query for all documents ("*:*")
        /// </summary>
		public static ISolrQuery All { get; private set; }
	}
}