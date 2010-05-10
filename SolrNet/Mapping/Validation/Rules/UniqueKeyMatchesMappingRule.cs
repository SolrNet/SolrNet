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
    /// Validation rule for making sure the uniqueKey mapped in the type is the same as in the Solr schema.
    /// </summary>
    public class UniqueKeyMatchesMappingRule : IValidationRule {
        /// <summary>
        /// Validates that the uniqueKey mapped in the type is the same as in the Solr schema.
        /// </summary>
        /// <param name="documentType">Document type</param>
        /// <param name="solrSchema">The solr schema.</param>
        /// <param name="mappingManager">The mapping manager.</param>
        /// <returns>
        /// A collection of <see cref="ValidationResult"/> objects with any issues found during validation.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(Type documentType, SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {
            var mappedKey = mappingManager.GetUniqueKey(documentType);
            if (mappedKey == null && solrSchema.UniqueKey == null)
                yield break;
            if (mappedKey == null && solrSchema.UniqueKey != null) {
                yield return new ValidationWarning(string.Format("Solr schema has unique key field '{0}' but mapped type '{1}' doesn't have a declared unique key", solrSchema.UniqueKey, documentType));
            } else if (mappedKey != null && solrSchema.UniqueKey == null) {
                yield return new ValidationError(string.Format("Type '{0}' has a declared unique key '{1}' but Solr schema doesn't have a unique key", documentType, mappedKey.FieldName));
            } else if (!mappedKey.FieldName.Equals(solrSchema.UniqueKey)) {
                yield return new ValidationError(String.Format("Solr schema unique key '{0}' does not match document unique key '{1}' in type '{2}'.", solrSchema.UniqueKey, mappedKey, documentType));
            }
        }
    }
}