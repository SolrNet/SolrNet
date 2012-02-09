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

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Commit/optimize options
    /// </summary>
	public class CommitOptions {
        /// <summary>
        /// Block until a new searcher is opened and registered as the main query searcher, making the changes visible. 
        /// Default is true
        /// </summary>
		public bool? WaitSearcher { get; set; }

        /// <summary>
        /// Block until index changes are flushed to disk
        /// Default is true
        /// </summary>
		public bool? WaitFlush { get; set; }

        /// <summary>
        /// Optimizes down to at most this number of segments
        /// Default is 1
        /// </summary>
        /// <remarks>Requires Solr 1.3</remarks>
        public int? MaxSegments { get; set; }

        /// <summary>
        /// Merge segments with deletes away
        /// </summary>
        /// <remarks>Requires Solr 1.4</remarks>
        public bool? ExpungeDeletes { get; set; }
	}
}