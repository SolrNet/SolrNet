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

namespace SolrNet.Mapping.Validation {
	/// <summary>
	/// Provides an interface to validation a Solr schema against a type's mapping.
	/// </summary>
	public interface IMappingValidationManager {
        /// <summary>
        /// Validates the specified validation rules.
        /// </summary>
        /// <typeparam name="T">The type of which the mapping needs to be validated</typeparam>
        /// <param name="solrSchemaXml">The Solr schema XML.</param>
        /// <param name="validationRules">The validation rules.</param>
        /// <returns>A collection of <see cref="MappingValidationItem"/> objects with the problems found during validation. If Any.</returns>
	    ICollection<MappingValidationItem> Validate<T>(XmlDocument solrSchemaXml, IEnumerable<Type> validationRules);

        /// <summary>
        /// Gets the validation rules.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>A collection of types implementing <see cref="IValidationRule"/>.</returns>
        ICollection<Type> GetValidationRules(IEnumerable<Type> types);
	}
}