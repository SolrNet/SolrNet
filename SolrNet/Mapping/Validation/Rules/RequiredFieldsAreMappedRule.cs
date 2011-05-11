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
    /// Represents a rule validating that all SolrFields in the SolrSchema which are required are
    /// either present in the mapping or as a CopyField.
    /// </summary>
    public class RequiredFieldsAreMappedRule : IValidationRule {
        /// <summary>
        /// Validates that all SolrFields in the SolrSchema which are required are
        /// either present in the mapping or as a CopyField.
        /// </summary>
        /// <param name="documentType">Document type</param>
        /// <param name="solrSchema">The solr schema.</param>
        /// <param name="mappingManager">The mapping manager.</param>
        /// <returns>
        /// A collection of <see cref="ValidationResult"/> objects with any issues found during validation.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(Type documentType, SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {

            foreach (SolrField solrField in solrSchema.SolrFields) {
                if (solrField.IsRequired) {
                    bool fieldFoundInMappingOrCopyFields = mappingManager.GetFields(documentType).ContainsKey(solrField.Name);

                    if (!fieldFoundInMappingOrCopyFields) {
                        foreach (SolrCopyField copyField in solrSchema.SolrCopyFields) {
                            if (copyField.Destination.Equals(solrField.Name)) {
                                fieldFoundInMappingOrCopyFields = true;
                                break;
                            }
                        }
                    }

                    if (!fieldFoundInMappingOrCopyFields)
                        yield return new ValidationError(String.Format("Required field '{0}' in the Solr schema is not mapped in type '{1}'.",
                                                     solrField.Name, documentType.FullName));
                }
            }
        }
    }
}