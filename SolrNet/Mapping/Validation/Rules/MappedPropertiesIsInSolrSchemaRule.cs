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
using System.Linq;
using System.Text.RegularExpressions;
using SolrNet.Schema;

namespace SolrNet.Mapping.Validation.Rules {
    /// <summary>
    /// Represents a rule validation that all properties in the mapping are present in the Solr schema
    /// as either a SolrField or a DynamicField
    /// </summary>
    public class MappedPropertiesIsInSolrSchemaRule : IValidationRule {
        /// <summary>
        /// Field names to be ignored. These fields are never checked.
        /// </summary>
        public ICollection<string> IgnoredFieldNames { get; set; }

        public MappedPropertiesIsInSolrSchemaRule() {
            IgnoredFieldNames = new[] {"score", "geo_distance"};
        }

        /// <summary>
        /// Validates that all properties in the mapping are present in the Solr schema
        /// as either a SolrField or a DynamicField
        /// </summary>
        /// <param name="documentType">Document type</param>
        /// <param name="solrSchema">The solr schema.</param>
        /// <param name="mappingManager">The mapping manager.</param>
        /// <returns>
        /// A collection of <see cref="ValidationResult"/> objects with any issues found during validation.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(Type documentType, SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {
            foreach (var mappedField in mappingManager.GetFields(documentType).Values) {
                var field = mappedField;
                if (IgnoredFieldNames != null && IgnoredFieldNames.Any(f => f == field.FieldName))
                    continue;
                if (field.FieldName.Contains("*")) // ignore multi-mapped fields (wildcards or dictionary mappings)
                    continue;
                var fieldFoundInSolrSchema = false;
                foreach (var solrField in solrSchema.SolrFields) {
                    if (solrField.Name.Equals(field.FieldName)) {
                        fieldFoundInSolrSchema = true;
                        break;
                    }
                }

                if (!fieldFoundInSolrSchema) {
                    foreach (var dynamicField in solrSchema.SolrDynamicFields) {
                        if (IsGlobMatch(dynamicField.Name, field.FieldName)) {
                            fieldFoundInSolrSchema = true;
                            break;
                        }
                    }
                }

                if (!fieldFoundInSolrSchema)
                    // If field couldn't be matched to any of the solrfield, dynamicfields throw an exception.
                    yield return new ValidationError(String.Format("No matching SolrField or DynamicField '{0}' found in the Solr schema for document property '{1}' in type '{2}'.",
                                                                          field.FieldName, field.Property.Name, documentType.FullName));
            }
        }

        private bool IsGlobMatch(string pattern, string input) {
            pattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
            var regex = new Regex(pattern);
            return regex.Match(input).Success;
        }
    }
}