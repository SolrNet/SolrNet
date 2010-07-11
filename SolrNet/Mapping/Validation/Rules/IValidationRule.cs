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
    /// Provides a interface to schema mapping validation rules.
    /// </summary>
    public interface IValidationRule {
        /// <summary>
        /// Validates the specified solr schema.
        /// </summary>
        /// <param name="propertyType">The type which mappings will be validated.</param>
        /// <param name="solrSchema">The solr schema.</param>
        /// <param name="mappingManager">The mapping manager.</param>
        /// <returns>A collection of <see cref="ValidationResult"/> objects with any issues found during validation.</returns>
        IEnumerable<ValidationResult> Validate(Type propertyType, SolrSchema solrSchema, IReadOnlyMappingManager mappingManager);
    }
}