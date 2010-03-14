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
using SolrNet.Schema;

namespace SolrNet.Mapping.Validation.Rules {
    public class IntSolrFieldTypeChecker : ISolrFieldTypeChecker
    {
        public IntSolrFieldTypeChecker()
        {
            SafeTypes = new List<string> { "solr.TrieIntField", "solr.IntField", "solr.SortableIntField" };
            WarningTypes = new List<string> { "solr.TextField", "solr.StrField" };
        }

        public List<string> SafeTypes { get; set; }
        public List<string> WarningTypes { get; set; }

        #region ISolrFieldTypeChecker Members

        public MappingValidationItem Validate(SolrFieldType solrFieldType, string propertyName, Type propertyType)
        {
            // Check if the type is in the safe or warning types and otherwise return a error
            if (SafeTypes != null && SafeTypes.Contains(solrFieldType.Type))
                return null;
            if (WarningTypes != null && WarningTypes.Contains(solrFieldType.Type))
                return new MappingValidationWarning(String.Format("Property '{0}' of type '{1}' is mapped to a solr field of type '{2}'. These types are not fully compatible.", propertyName, propertyType.FullName, solrFieldType.Name));

            return new MappingValidationError(String.Format("Property '{0}' of type '{1}' cannot be storred in solr field type '{2}'.", propertyName, propertyType.FullName, solrFieldType.Name));
        }

        #endregion
    }
}