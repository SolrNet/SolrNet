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

namespace SolrNet.Mapping.Validation {
	/// <summary>
	/// Provides an interface to validation a Solr schema against a type's mapping.
	/// </summary>
	public interface IMappingValidator {
        /// <summary>
        /// Validates the specified validation rules.
        /// </summary>
        /// <param name="documentType">The document type which needs to be validated</param>
        /// <param name="schema">The Solr schema.</param>
        /// <returns>A collection of <see cref="ValidationResult"/> objects with the problems found during validation. If Any.</returns>
	    IEnumerable<ValidationResult> EnumerateValidationResults(Type documentType, SolrSchema schema);
	}
}