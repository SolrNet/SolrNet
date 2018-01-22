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

using System;
using System.Collections.Generic;

namespace SolrNet.Impl {
    /// <summary>
    /// Collation Result
    /// </summary>
    public class CollationResult {

        /// <summary>
        /// Number of hits
        /// </summary>
        private long _hits;

        /// <summary>
        /// Constructor
        /// </summary>
        internal CollationResult()
        {
            MisspellingsAndCorrections = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Collation query term
        /// </summary>
        public string CollationQuery { get; internal set;}

        /// <summary>
        /// Number of hits
        /// </summary>
        public long Hits {
            get {
                if (_hits >= 0)
                {
                    return _hits;
                }
                else
                {
                    throw new InvalidOperationException("Operation not supported when collateExtendedResults parameter is set to false.");
                }
            }
            internal set
            {
                _hits = value;
            }
        }

        /// <summary>
        /// MisspellingsAndCorrections
        /// </summary>
        public ICollection<KeyValuePair<string, string>> MisspellingsAndCorrections { get; internal set;}
    }
}