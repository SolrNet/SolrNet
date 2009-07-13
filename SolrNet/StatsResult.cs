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
    public class StatsResult {
        //public double Name { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Sum { get; set; }
        public long Count { get; set; }
        public long Missing { get; set; }
        public double SumOfSquares { get; set; }
        public double Mean { get; set; }
        public double StdDev { get; set; }

        /// <summary>
        /// Recursive list in case of facets
        /// </summary>
        public IDictionary<string, Dictionary<string, StatsResult>> FacetResults { get; set; }

        public StatsResult() {
            FacetResults = new Dictionary<string, Dictionary<string, StatsResult>>();
        }
    }
}