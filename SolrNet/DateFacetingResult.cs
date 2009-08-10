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
using System;

namespace SolrNet {
    /// <summary>
    /// Date faceting result
    /// <see cref="http://wiki.apache.org/solr/SimpleFacetParameters#head-068dc96b0dac1cfc7264fe85528d7df5bf391acd"/>
    /// </summary>
    public class DateFacetingResult {

		/// <summary>
		/// The gap.
		/// </summary>
        public string Gap { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public DateTime End { get; set; }

		/// <summary>
		/// The date faceting results.
		/// </summary>
		public IDictionary<string, Int64> DateResults { get; set; }

		public DateFacetingResult() {
			DateResults = new Dictionary<string, Int64>();
        }
    }
}