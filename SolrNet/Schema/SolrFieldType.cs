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

namespace SolrNet.Schema {
    /// <summary>
    /// Represents a Solr field type.
    /// </summary>
    public class SolrFieldType {
        /// <summary>
        /// Represents a Solr field type.
        /// </summary>
        /// <param name="name">Solr field type name</param>
        /// <param name="type">Solr field type type (java class name)</param>
        public SolrFieldType(string name, string type) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (type == null)
                throw new ArgumentNullException("type");
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; private set; }
    }
}
