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

namespace SolrNet {
    /// <summary>
    /// Stats results
    /// <see href="http://wiki.apache.org/solr/StatsComponent"/>
    /// </summary>
    public class StatsResult
    {
        private readonly TypedStatsResult<string> stringValues;
        private readonly TypedStatsResult<double> doubleValues;

        /// <summary>
        /// Minimum value
        /// </summary>
        public double Min
        {
            get => doubleValues.Min;
            set => doubleValues.Min = value;
        }

        /// <summary>
        /// Maximum value
        /// </summary>
        public double Max
        {
            get => doubleValues.Max;
            set => doubleValues.Max = value;
        }

        /// <summary>
        /// Sum of all values
        /// </summary>
        public double Sum
        {
            get => doubleValues.Sum;
            set => doubleValues.Sum = value;
        }

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
        public double SumOfSquares
        {
            get => doubleValues.SumOfSquares;
            set => doubleValues.SumOfSquares = value;
        }

        /// <summary>
        /// The average (v1+v2...+vN)/N
        /// </summary>
        public double Mean
        {
            get => doubleValues.Mean;
            set => doubleValues.Mean = value;
        }

        /// <summary>
        /// Standard deviation
        /// </summary>
        public double StdDev
        {
            get => doubleValues.StdDev;
            set => doubleValues.StdDev = value;
        }

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
        /// Returns a `TypedStatsResult`.
        ///
        /// In case T is string, the instance's `TypedStatsResultString` is returned.
        /// In case T is double, the instance's `TypedStatsResultDouble` is returned.
        /// Otherwise a `TypedStatsResultCast` is returned.
        /// </summary>
        /// <typeparam name="T">The type to cast the `StatsResult` to.</typeparam>
        public TypedStatsResult<T> GetTyped<T>()
        {
            var type = typeof(T);
            if (type == typeof(string))
                return stringValues as TypedStatsResult<T>;
            if (type == typeof(double))
                return doubleValues as TypedStatsResult<T>;
            return new TypedStatsResultCast<T>(stringValues);
        }
        
        /// <summary>
        /// Stats results
        /// </summary>
        public StatsResult(TypedStatsResult<string> stringValues)
        {
            this.stringValues = stringValues;
            doubleValues = new TypedStatsResultDouble(stringValues);
            FacetResults = new Dictionary<string, Dictionary<string, StatsResult>>();
        }
    }
}
