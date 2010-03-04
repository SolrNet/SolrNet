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
using System.Text.RegularExpressions;
using SolrNet.Schema;

namespace SolrNet.Mapping.Validation.Rules {
    /// <summary>
    /// Represents a rule validation that all properties in the mapping are present in the Solr schema
    /// as either a SolrField or a DynamicField
    /// </summary>
    public class MappedPropertiesIsInSolrSchemaRule : IValidationRule {
        /// <summary>
        /// Validates that all properties in the mapping are present in the Solr schema
        /// as either a SolrField or a DynamicField
        /// </summary>
        /// <typeparam name="T">The type which mappings will be validated.</typeparam>
        /// <param name="solrSchema">The solr schema.</param>
        /// <param name="mappingManager">The mapping manager.</param>
        /// <returns>
        /// A collection of <see cref="MappingValidationItem"/> objects with any issues found during validation.
        /// </returns>
        public IEnumerable<MappingValidationItem> Validate<T>(SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {
            foreach (var mappedField in mappingManager.GetFields(typeof (T))) {
                var fieldFoundInSolrSchema = false;
                foreach (var solrField in solrSchema.SolrFields) {
                    if (solrField.Name.Equals(mappedField.Value)) {
                        fieldFoundInSolrSchema = true;
                        break;
                    }
                }

                if (!fieldFoundInSolrSchema) {
                    foreach (var dynamicField in solrSchema.SolrDynamicFields) {
                        if (IsGlobMatch(dynamicField.Name, mappedField.Value)) {
                            fieldFoundInSolrSchema = true;
                            break;
                        }
                    }
                }

                if (!fieldFoundInSolrSchema)
                    // If field couldn't be matched to any of the solrfield, dynamicfields throw an exception.
                    yield return new MappingValidationError(String.Format("No matching SolrField or DynamicField found in the Solr schema for document property '{0}' in type '{1}'.",
                                                                          mappedField.Key.Name, typeof (T).FullName));
            }
        }

        private bool IsGlobMatch(string pattern, string input) {
            pattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
            var regex = new Regex(pattern);
            return regex.Match(input).Success;
        }
    }
}