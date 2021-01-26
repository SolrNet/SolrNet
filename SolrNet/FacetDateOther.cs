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

namespace SolrNet {
    /// <summary>
    /// Indicates that in addition to the counts for each date range constraint between facet.date.start and facet.date.end, 
    /// counts should also be computed for other
    /// </summary>
    public class FacetDateOther {
        protected readonly string value;

        /// <summary>
        /// Indicates that in addition to the counts for each date range constraint between facet.date.start and facet.date.end, 
        /// counts should also be computed for other
        /// </summary>
        protected FacetDateOther(string value) {
            this.value = value;
        }

        /// <summary>
        /// All records with field values lower then lower bound of the first range
        /// </summary>
        public static FacetDateOther Before {
            get { return new FacetDateOther("before"); }
        }

        /// <summary>
        /// All records with field values greater then the upper bound of the last range
        /// </summary>
        public static FacetDateOther After {
            get { return new FacetDateOther("after"); }
        }

        /// <summary>
        /// All records with field values between the start and end bounds of all ranges
        /// </summary>
        public static FacetDateOther Between {
            get { return new FacetDateOther("between"); }
        }

        /// <summary>
        /// Compute none of this information. Overrides all other options.
        /// </summary>
        public static FacetDateOther None {
            get { return new FacetDateOther("none"); }
        }

        /// <summary>
        /// Shortcut for before, between, and after
        /// </summary>
        public static FacetDateOther All {
            get { return new FacetDateOther("all"); }
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            var o = obj as FacetDateOther;
            if (o == null)
                return false;
            return o.value == value;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString() {
            return value;
        }
    }
}
