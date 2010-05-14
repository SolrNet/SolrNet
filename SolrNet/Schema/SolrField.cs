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

namespace SolrNet.Schema
{
    /// <summary>
    /// Repesents a field in the Solr schema.
    /// </summary>
    public class SolrField
    {
        /// <summary>
        /// Repesents a field in the Solr schema.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="type">Field type</param>
        public SolrField(string name, SolrFieldType type) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (type == null)
                throw new ArgumentNullException("type");
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Field name
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi valued.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is multi valued; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultiValued { get; set; }

        /// <summary>
        /// Field type
        /// </summary>
        /// <value>The type.</value>
        public SolrFieldType Type { get; private set; }
    }
}
