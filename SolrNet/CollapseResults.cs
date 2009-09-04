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

namespace SolrNet {
    /// <summary>
    /// Collapse results
    /// <see cref="https://issues.apache.org/jira/browse/SOLR-236"/>
    /// </summary>
    public class CollapseResults {
        
        /// <summary>
        /// &collapse.field=
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Collapsed document.ids and their counts
        /// </summary>
        public IDictionary<string, int> DocResults { get; set; }

        /// <summary>
        /// Collapsed field values and their counts
        /// </summary>
        public IDictionary<string, int> CountResults { get; set; }

        public CollapseResults() {
            DocResults = new Dictionary<string, int>();
            CountResults = new Dictionary<string, int>();
        }
    }
}