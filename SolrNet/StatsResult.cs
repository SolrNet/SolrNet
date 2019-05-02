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
using static SolrNet.Impl.ResponseParsers.StatsResponseParser<double>;

namespace SolrNet {
    /// <summary>
    /// Stats results
    /// <see href="http://wiki.apache.org/solr/StatsComponent"/>
    /// </summary>
    public class StatsResult
    {
        private readonly ITypedStatsResult<string> stringValues;
        
        private double min;
        private double max;
        private double sum;
        private double sumOfSquares;
        private double mean;
        private double stdDev;

        /// <summary>
        /// Minimum value
        /// </summary>
        [Obsolete("Deprecated, please use `GetTyped<double>().Min` instead.")]
        public double Min
        {
            get
            {
                if (min == default(double))
                    min = GetDoubleValue(stringValues.Min);
                return min;
            }
            set => min = value;
        }

        /// <summary>
        /// Maximum value
        /// </summary>
        [Obsolete("Deprecated, please use `GetTyped<double>().Max` instead.")]
        public double Max
        {
            get
            {
                if (max == default(double))
                    max = GetDoubleValue(stringValues.Max);
                return max;
            }
            set => max = value;
        }

        /// <summary>
        /// Sum of all values
        /// </summary>
        [Obsolete("Deprecated, please use `GetTyped<double>().Sum` instead.")]
        public double Sum
        {
            get
            {
                if (sum == default(double))
                    sum = GetDoubleValue(stringValues.Sum);
                return sum;
            }
            set => sum = value;
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
        [Obsolete("Deprecated, please use `GetTyped<double>().SumOfSquares` instead.")]
        public double SumOfSquares
        {
            get
            {
                if (sumOfSquares == default(double))
                    sumOfSquares = GetDoubleValue(stringValues.SumOfSquares);
                return sumOfSquares;
            }
            set => sumOfSquares = value;
        }

        /// <summary>
        /// The average (v1+v2...+vN)/N
        /// </summary>
        [Obsolete("Deprecated, please use `GetTyped<double>().Mean` instead.")]
        public double Mean
        {
            get
            {
                if (mean == default(double))
                    mean = GetDoubleValue(stringValues.Mean);
                return mean;
            }
            set => mean = value;
        }

        /// <summary>
        /// Standard deviation
        /// </summary>
        [Obsolete("Deprecated, please use `GetTyped<double>().StdDev` instead.")]
        public double StdDev
        {
            get
            {
                if (stdDev == default(double))
                    stdDev = GetDoubleValue(stringValues.StdDev);
                return stdDev;
            }
            set => stdDev = value;
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
        /// Otherwise a `TypedStatsResultCast` is returned.
        /// </summary>
        /// <typeparam name="T">The type to cast the `StatsResult` properties to.</typeparam>
        public ITypedStatsResult<T> GetTyped<T>()
        {
            var type = typeof(T);
            if (type == typeof(string))
                return stringValues as ITypedStatsResult<T>;
            return new TypedStatsResultCast<T>(stringValues);
        }
        
        /// <summary>
        /// Stats results
        /// </summary>
        public StatsResult(ITypedStatsResult<string> stringValues)
        {
            this.stringValues = stringValues;
            FacetResults = new Dictionary<string, Dictionary<string, StatsResult>>();
        }
    }
}
