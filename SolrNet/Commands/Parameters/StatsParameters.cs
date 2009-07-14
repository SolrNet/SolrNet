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

using System;
using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    public class StatsParameters {
        public IDictionary<string, ICollection<string>> FieldsWithFacets { get; set; }

        public ICollection<string> Facets { get; set; }

        public StatsParameters AddFacet(string facet) {
            Facets.Add(facet);
            return this;
        }

        public StatsParameters AddField(string field) {
            if (field == null)
                throw new ArgumentNullException("field");
            FieldsWithFacets[field] = new List<string>();
            return this;
        }

        public StatsParameters AddFieldWithFacet(string field, string facet) {
            if (field == null)
                throw new ArgumentNullException("field");
            FieldsWithFacets[field] = new List<string> { facet };
            return this;
        }

        public StatsParameters AddFieldWithFacets(string field, IEnumerable<string> facets) {
            if (field == null)
                throw new ArgumentNullException("field");
            if (facets == null)
                throw new ArgumentNullException("facets");
            FieldsWithFacets[field] = new List<string>(facets);
            return this;
        }

        public StatsParameters AddFieldWithFacets(string field, params string[] facets) {
            return AddFieldWithFacets(field, (IEnumerable<string>)facets);
        }

        public StatsParameters() {
            FieldsWithFacets = new Dictionary<string, ICollection<string>>();
            Facets = new List<string>();
        }
    }
}