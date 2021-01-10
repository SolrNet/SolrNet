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

namespace SolrNet.Mapping.Validation.Rules {
    /// <summary>
    /// Checks schema type of properties with <see cref="int"/> type
    /// </summary>
    public class IntSolrFieldTypeChecker : AbstractSolrFieldTypeChecker {
        /// <summary>
        /// Checks schema type of properties with <see cref="int"/> type
        /// </summary>
        public IntSolrFieldTypeChecker()
            : base(new[] {"solr.TrieIntField", "solr.IntField", "solr.SortableIntField"},
                   new[] {"solr.TextField", "solr.StrField"}) {}

        /// <inheritdoc />
        public override bool CanHandleType(Type propertyType) {
            return propertyType == typeof (int) ||
                propertyType == typeof(int?);
        }
    }
}
