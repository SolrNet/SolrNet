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
        private readonly IValidationRule[] rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingValidationManager"/> class.
        /// </summary>
        /// <param name="mappingManager">The mapping manager that is used to map types to and from their Solr representation.</param>
        /// <param name="schemaParser">The schema parser to use to parse the Solr schema XML.</param>
        /// <param name="rules">Validation rules</param>
        public MappingValidationManager(IReadOnlyMappingManager mappingManager, ISolrSchemaParser schemaParser, IValidationRule[] rules) {
            this.mappingManager = mappingManager;
            this.schemaParser = schemaParser;
            this.rules = rules;
        }

        /// <summary>
        /// Validates the specified validation rules.
        /// </summary>
        /// <typeparam name="T">The type of which the mapping needs to be validated</typeparam>
        /// <param name="solrSchemaXml">The Solr schema XML.</param>
        /// <returns>A collection of <see cref="MappingValidationItem"/> objects with the problems found during validation. If Any.</returns>
        public IEnumerable<MappingValidationItem> Validate<T>(XmlDocument solrSchemaXml) {
            var solrSchema = schemaParser.Parse(solrSchemaXml);

            foreach (var rule in rules) {
                var items = rule.Validate<T>(solrSchema, mappingManager);
                foreach (var i in items)
                    yield return i;
            }
        }
    }
}