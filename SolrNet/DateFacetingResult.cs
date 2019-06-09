﻿#region license
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
using System;

namespace SolrNet {
    /// <summary>
    /// Date faceting result
    /// </summary>
    [Obsolete("As of Solr 3.1 has been deprecated, as of Solr 6.6 unsupported.")]
    public class DateFacetingResult {

		/// <summary>
		/// Date range gap (e.g. "+1DAY")
		/// </summary>
        public string Gap { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public DateTime End { get; set; }

		/// <summary>
        /// Minimum value
        /// </summary>
        public DateTime Start { get; set; }
		/// <summary>
		/// The date faceting results.
		/// </summary>
		public IList<KeyValuePair<DateTime, int>> DateResults { get; set; }

        /// <summary>
        /// Other date faceting results.
        /// </summary>
        public IDictionary<FacetDateOther, int> OtherResults { get; set; }

        /// <summary>
        /// Date faceting result
        /// </summary>
		public DateFacetingResult() {
			DateResults = new List<KeyValuePair<DateTime, int>>();
            OtherResults = new Dictionary<FacetDateOther, int>();
        }
    }
}