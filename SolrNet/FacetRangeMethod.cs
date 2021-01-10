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
    /// The Facet Range Method selects the type of algorithm or method Solr should use for range faceting. Both methods produce the same results, but performance may vary.
    /// </summary>
    public class FacetRangeMethod {
        protected readonly string value;

        protected FacetRangeMethod(string value) {
            this.value = value;
        }

        /// <summary>
        /// This method generates the ranges based on other facet.range parameters, and for each of them executes a filter that later intersects with the main query resultset to get the count. It will make use of the filterCache, so it will benefit of a cache large enough to contain all ranges.
        /// </summary>
        public static FacetRangeMethod Filter {
            get { return new FacetRangeMethod("filter"); }
        }

        /// <summary>
        /// This method iterates the documents that match the main query, and for each of them finds the correct range for the value. This method will make use of docValues (if enabled for the field) or fieldCache. The dv method is not supported for field type DateRangeField or when using group.facets.
        /// </summary>
        public static FacetRangeMethod DV {
            get { return new FacetRangeMethod("dv"); }
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            var o = obj as FacetRangeMethod;
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
