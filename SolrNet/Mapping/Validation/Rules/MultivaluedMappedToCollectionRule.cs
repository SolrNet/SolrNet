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
        /// <param name="documentType">Document type</param>
        /// <param name="solrSchema">The solr schema.</param>
        /// <param name="mappingManager">The mapping manager.</param>
        /// <returns>
        /// A collection of <see cref="ValidationResult"/> objects with any issues found during validation.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(Type documentType, SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {
            var collectionFieldParser = new CollectionFieldParser(null); // Used to check if the type is a collection type.

            foreach (var prop in mappingManager.GetFields(documentType)) {
                var solrField = solrSchema.FindSolrFieldByName(prop.Key);
                if (solrField == null)
                    continue;
                var isCollection = collectionFieldParser.CanHandleType(prop.Value.Property.PropertyType);
                if (solrField.IsMultiValued && !isCollection)
                    yield return new ValidationError(String.Format("SolrField '{0}' is multivalued while property '{1}.{2}' is not mapped as a collection.", solrField.Name, prop.Value.Property.DeclaringType, prop.Value.Property.Name));
                else if (!solrField.IsMultiValued && isCollection)
                    yield return new ValidationError(String.Format("SolrField '{0}' is not multivalued while property '{1}.{2}' is mapped as a collection.", solrField.Name, prop.Value.Property.DeclaringType, prop.Value.Property.Name));
            }
        }
    }
}