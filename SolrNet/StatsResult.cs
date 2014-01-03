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
    public abstract class StatsResultValues {
        public abstract T Match<T>(Func<StatsResultTypedValues<double>, T> ifDouble, Func<StatsResultTypedValues<string>, T> ifString, Func<StatsResultTypedValues<DateTimeOffset>, T> ifDate);
    }

    public abstract class StatsResultTypedValues<T> : StatsResultValues {
        public T Min { get; set; }
        public T Max { get; set; }
        public T Sum { get; set; }
        public T Mean { get; set; }

        private StatsResultTypedValues() { }

        public class StatsResultTypedValuesDouble : StatsResultTypedValues<double> {
            public override T Match<T>(Func<StatsResultTypedValues<double>, T> ifDouble, Func<StatsResultTypedValues<string>, T> ifString, Func<StatsResultTypedValues<DateTimeOffset>, T> ifDate) {
                return ifDouble(this);
            }
        }

        public class StatsResultTypedValuesString : StatsResultTypedValues<string> {
            public override T Match<T>(Func<StatsResultTypedValues<double>, T> ifDouble, Func<StatsResultTypedValues<string>, T> ifString, Func<StatsResultTypedValues<DateTimeOffset>, T> ifDate) {
                return ifString(this);
            }
        }

        public class StatsResultTypedValuesDate : StatsResultTypedValues<DateTimeOffset> {
            public override T Match<T>(Func<StatsResultTypedValues<double>, T> ifDouble, Func<StatsResultTypedValues<string>, T> ifString, Func<StatsResultTypedValues<DateTimeOffset>, T> ifDate) {
                return ifDate(this);
            }
        }
    }

    /// <summary>
    /// Stats results
    /// <see href="http://wiki.apache.org/solr/StatsComponent"/>
    /// </summary>
    public sealed class StatsResult {
        public StatsResultValues Values { get; set; }

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
        public double SumOfSquares { get; set; }

        /// <summary>
        /// Standard deviation
        /// </summary>
        public double StdDev { get; set; }

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
        /// Stats results
        /// </summary>
        public StatsResult() {
            FacetResults = new Dictionary<string, Dictionary<string, StatsResult>>();
        }
    }
}