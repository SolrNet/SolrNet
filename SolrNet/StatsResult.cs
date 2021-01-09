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

namespace SolrNet {
    /// <summary>
    /// Stats results
    /// <see href="http://wiki.apache.org/solr/StatsComponent"/>
    /// </summary>
    public class StatsResult {
        
        private readonly ITypedStatsResult<string> statsResult;
        
        /// <summary>
        /// Minimum value
        /// </summary>
        [Obsolete("Use `AsType<double?>().Min` instead.")]
        public double Min { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        [Obsolete("Use `AsType<double?>().Max` instead.")]
        public double Max { get; set; }

        /// <summary>
        /// Sum of all values
        /// </summary>
        [Obsolete("Use `AsType<double?>().Sum` instead.")]
        public double Sum { get; set; }

        /// <summary>
        /// How many (non-null) values
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// How many null values
        /// </summary>
        public long Missing { get; set; }

        /// <summary>
        /// Sum of all values squared (useful for stddev)
        /// </summary>
        [Obsolete("Use `AsType<double?>().SumOfSquares` instead.")]
        public double SumOfSquares { get; set; }

        /// <summary>
        /// The average (v1+v2...+vN)/N
        /// </summary>
        [Obsolete("Use `AsType<double?>().Mean` instead.")]
        public double Mean { get; set; }

        /// <summary>
        /// Standard deviation
        /// </summary>
        [Obsolete("Use `AsType<double?>().StdDev` instead.")]
        public double StdDev { get; set; }

        /// <summary>
        /// A list of percentile values based on cut-off points specified.
        /// </summary>
        public IDictionary<double, double> Percentiles { get; set; }

        /// <summary>
        /// Facet results.
        /// <list type="bullet">
        /// <item>Key is the facet field</item>
        /// <item>Value is a dictionary where:
        /// <list type="bullet">
        /// <item>Key is the facet value</item>
        /// <item>Value is the stats for the facet value</item>
        /// </list>
        /// </item>
        /// </list>
        /// </summary>
        public IDictionary<string, Dictionary<string, StatsResult>> FacetResults { get; set; }

        /// <summary>
        /// Returns type specific properties of this stats instance (like Min, Max) converted to the specified type T.
        /// </summary>
        /// <typeparam name="T">A ITypedStatsResult containing the type specific properties of this stats instance.</typeparam>
        public ITypedStatsResult<T> AsType<T>()
        {
            var type = typeof(T);
            if (type == typeof(string))
                return statsResult as ITypedStatsResult<T>;
            return new TypedStatsResult<T>(statsResult);
        }
        
        /// <summary>
        /// Stats results
        /// </summary>
        public StatsResult(ITypedStatsResult<string> statsResult)
        {
            this.statsResult = statsResult;
            FacetResults = new Dictionary<string, Dictionary<string, StatsResult>>();
        }
    }
}
