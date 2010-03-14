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

namespace SolrNet.Impl {
    /// <summary>
    /// Spell-checking results
    /// </summary>
    public class SpellCheckResult {
        /// <summary>
        /// Original query term
        /// </summary>
        public string Query { get; set;}

        /// <summary>
        /// Result count for original term
        /// </summary>
        public int NumFound { get; set;}

        /// <summary>
        /// Start offset
        /// </summary>
        public int StartOffset { get; set;}

        /// <summary>
        /// End offset
        /// </summary>
        public int EndOffset { get; set;}

        /// <summary>
        /// Spelling suggestions
        /// </summary>
        public ICollection<string> Suggestions { get; set;}
    }
}