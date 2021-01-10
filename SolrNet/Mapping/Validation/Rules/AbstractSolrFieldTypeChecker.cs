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
    /// <summary>
    /// Abstract schema type checker. Uses a list of "safe" types and a list of "warning" types
    /// </summary>
    public abstract class AbstractSolrFieldTypeChecker : ISolrFieldTypeChecker {
        protected readonly ICollection<string> safeTypes;
        protected readonly ICollection<string> warningTypes;

        protected AbstractSolrFieldTypeChecker(ICollection<string> safeTypes, ICollection<string> warningTypes) {
            this.safeTypes = safeTypes;
            this.warningTypes = warningTypes;
        }

        /// <inheritdoc/>
        public virtual ValidationResult Validate(SolrFieldType solrFieldType, string propertyName, Type propertyType) {
            // Check if the type is in the safe or warning types and otherwise return a error
            if (safeTypes != null && safeTypes.Contains(solrFieldType.Type))
                return null;
            if (warningTypes != null && warningTypes.Contains(solrFieldType.Type))
                return new ValidationWarning(String.Format("Property '{0}' of type '{1}' is mapped to a solr field of type '{2}'. These types are not fully compatible.", propertyName, propertyType.FullName, solrFieldType.Name));

            return new ValidationError(String.Format("Property '{0}' of type '{1}' cannot be stored in solr field type '{2}'.", propertyName, propertyType.FullName, solrFieldType.Name));
        }

        /// <summary>
        /// Returns true if this type checked can handle <paramref name="propertyType"/>
        /// </summary>
        /// <param name="propertyType">Type to check if this checker can handle</param>
        /// <returns></returns>
        public abstract bool CanHandleType(Type propertyType);
    }
}
