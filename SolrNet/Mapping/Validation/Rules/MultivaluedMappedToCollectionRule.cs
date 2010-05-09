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
using SolrNet.Impl.FieldParsers;
using SolrNet.Schema;

namespace SolrNet.Mapping.Validation.Rules {
    /// <summary>
    /// Represents a rule that validates that fields mapped to a solr field with mutilvalued set to true
    /// are of a type that implements <see cref="IEnumerable{T}" />.
    /// </summary>
    public class MultivaluedMappedToCollectionRule : IValidationRule {
        /// <summary>
        /// Validates the specified the mapped document against the solr schema.
        /// </summary>
        /// <typeparam name="T">The type which mappings will be validated.</typeparam>
        /// <param name="solrSchema">The solr schema.</param>
        /// <param name="mappingManager">The mapping manager.</param>
        /// <returns>
        /// A collection of <see cref="MappingValidationItem"/> objects with any issues found during validation.
        /// </returns>
        public IEnumerable<MappingValidationItem> Validate(Type propertyType, SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {
            var collectionFieldParser = new CollectionFieldParser(null); // Used to check if the type is a collection type.

            foreach (var prop in mappingManager.GetFields(propertyType)) {
                if (collectionFieldParser.CanHandleType(prop.Property.PropertyType)) { // Field is a collection so solr field should be too.
                    var solrField = solrSchema.FindSolrFieldByName(prop.FieldName);
                    if (solrField != null) {
                        if(!solrField.IsMultiValued) {
                            yield return new MappingValidationError(String.Format("SolrField '{0}' is not multivalued while mapped type '{1}' implements IEnumberable<T>.", solrField.Name, prop.Property.Name));
                        }
                    }
                } else { //Mapped type is not a collection so solr field shouldn't be either.
                    var solrField = solrSchema.FindSolrFieldByName(prop.FieldName);
                    if (solrField != null)
                    {
                        if (solrField.IsMultiValued)
                        {
                            yield return new MappingValidationError(String.Format("SolrField '{0}' is multivalued while mapped type '{1}' does not implement IEnumberable<T>.", solrField.Name, prop.Property.Name));
                        }
                    }
                }
            }
        }
    }
}