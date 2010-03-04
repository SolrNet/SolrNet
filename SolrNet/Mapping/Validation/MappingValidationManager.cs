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
using System.Xml;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;

namespace SolrNet.Mapping.Validation {
    /// <summary>
    /// Manages the validation of a mapping against a solr schema XML document.
    /// </summary>
    public class MappingValidationManager : IMappingValidationManager {
        private readonly IReadOnlyMappingManager mappingManager;
        private readonly ISolrSchemaParser schemaParser;

        private SolrSchema solrSchema;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingValidationManager"/> class.
        /// </summary>
        /// <param name="mappingManager">The mapping manager that is used to map types to and from their Solr representation.</param>
        /// <param name="schemaParser">The schema parser to use to parse the Solr schema XML.</param>
        public MappingValidationManager(IReadOnlyMappingManager mappingManager, ISolrSchemaParser schemaParser) {
            this.mappingManager = mappingManager;
            this.schemaParser = schemaParser;
        }

        #region IMappingValidationManager Members

        /// <summary>
        /// Validates the specified validation rules.
        /// </summary>
        /// <typeparam name="T">The type of which the mapping needs to be validated</typeparam>
        /// <param name="solrSchemaXml">The Solr schema XML.</param>
        /// <param name="validationRules">The validation rules.</param>
        /// <returns>A collection of <see cref="MappingValidationItem"/> objects with the problems found during validation. If Any.</returns>
        public ICollection<MappingValidationItem> Validate<T>(XmlDocument solrSchemaXml, IEnumerable<Type> validationRules) {
            solrSchema = schemaParser.Parse(solrSchemaXml);

            var result = new List<MappingValidationItem>();
            foreach (Type type in GetValidationRules(validationRules)) {
                var validationRule = (IValidationRule) Activator.CreateInstance(type);
                result.AddRange(validationRule.Validate<T>(solrSchema, mappingManager));
            }
            return result;
        }

        /// <summary>
        /// Gets the validation rules.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>A collection of types implementing <see cref="IValidationRule"/>.</returns>
        public ICollection<Type> GetValidationRules(IEnumerable<Type> types) {
            var result = new List<Type>();
            foreach (Type type in types)
                if (typeof (IValidationRule).IsAssignableFrom(type))
                    result.Add(type);

            return result;
        }

        #endregion
    }
}