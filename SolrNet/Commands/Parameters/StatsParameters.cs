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

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Parameters to query stats
    /// <see href="http://wiki.apache.org/solr/StatsComponent"/>
    /// </summary>
    public class StatsParameters {
        /// <summary>
        /// Dictionary of fields to get stats, and their associated facets (if any)
        /// </summary>
        public IDictionary<string, ICollection<string>> FieldsWithFacets { get; set; }

        /// <summary>
        /// Global facets: get these facets' stats for all fields requested in <see cref="FieldsWithFacets"/>
        /// </summary>
        public ICollection<string> Facets { get; set; }

        /// <summary>
        /// Adds a facet to the <see cref="Facets"/> collection
        /// </summary>
        /// <param name="facet"></param>
        /// <returns></returns>
        public StatsParameters AddFacet(string facet) {
            Facets.Add(facet);
            return this;
        }

        /// <summary>
        /// Adds a field (without facets) to the <see cref="FieldsWithFacets"/> collection
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public StatsParameters AddField(string field) {
            if (field == null)
                throw new ArgumentNullException("field");
            FieldsWithFacets[field] = new List<string>();
            return this;
        }

        /// <summary>
        /// Adds a field with a related facet to the <see cref="FieldsWithFacets"/> collection
        /// </summary>
        /// <param name="field"></param>
        /// <param name="facet"></param>
        /// <returns></returns>
        public StatsParameters AddFieldWithFacet(string field, string facet) {
            if (field == null)
                throw new ArgumentNullException("field");
            FieldsWithFacets[field] = new List<string> { facet };
            return this;
        }

        /// <summary>
        /// Adds a field with related facets to the <see cref="FieldsWithFacets"/> collection
        /// </summary>
        /// <param name="field"></param>
        /// <param name="facets"></param>
        /// <returns></returns>
        public StatsParameters AddFieldWithFacets(string field, IEnumerable<string> facets) {
            if (field == null)
                throw new ArgumentNullException("field");
            if (facets == null)
                throw new ArgumentNullException("facets");
            FieldsWithFacets[field] = new List<string>(facets);
            return this;
        }

        /// <summary>
        /// Adds a field with related facets to the <see cref="FieldsWithFacets"/> collection
        /// </summary>
        /// <param name="field"></param>
        /// <param name="facets"></param>
        /// <returns></returns>
        public StatsParameters AddFieldWithFacets(string field, params string[] facets) {
            return AddFieldWithFacets(field, (IEnumerable<string>)facets);
        }

        /// <summary>
        /// Parameters to query stats
        /// </summary>
        public StatsParameters() {
            FieldsWithFacets = new Dictionary<string, ICollection<string>>();
            Facets = new List<string>();
        }
    }
}