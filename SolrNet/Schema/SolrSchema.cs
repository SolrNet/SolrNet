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
using System.Linq;
using SolrNet.Utils;

namespace SolrNet.Schema {
    /// <summary>
    /// Represents a Solr schema.
    /// </summary>
    public class SolrSchema {
        /// <summary>
        /// Gets or sets the solr fields types.
        /// </summary>
        /// <value>The solr fields types.</value>
        public List<SolrFieldType> SolrFieldTypes { get; set; }

        /// <summary>
        /// Gets or sets the solr fields.
        /// </summary>
        /// <value>The solr fields.</value>
        public List<SolrField> SolrFields { get; set; }

        /// <summary>
        /// Gets or sets the solr dynamic fields.
        /// </summary>
        /// <value>The solr dynamic fields.</value>
        public List<SolrDynamicField> SolrDynamicFields { get; set; }

        /// <summary>
        /// Gets or sets the solr copy fields.
        /// </summary>
        /// <value>The solr copy fields.</value>
        /// 
        public List<SolrCopyField> SolrCopyFields { get; set; }

        /// <summary>
        /// Gets or sets the unique key.
        /// </summary>
        /// <value>The unique key.</value>
        public string UniqueKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrSchema"/> class.
        /// </summary>
        public SolrSchema() {
            SolrFieldTypes = new List<SolrFieldType>();
            SolrFields = new List<SolrField>();
            SolrDynamicFields = new List<SolrDynamicField>();
            SolrCopyFields = new List<SolrCopyField>();
        }

        /// <summary>
        /// Finds the solr field by name.
        /// </summary>
        /// <param name="name">The name of the solr field to find.</param>
        /// <returns>The solr field if found. Null otherwise.</returns>
        public SolrField FindSolrFieldByName(string name) {
            foreach(var field in SolrFields) {
                if (field.Name.Equals(name))
                    return field;
            }
            return null;
        }

        public SolrFieldType FindSolrFieldTypeByName(string name) {
            return SolrFieldTypes.FirstOrDefault(t => t.Name == name);
        }
    }
}