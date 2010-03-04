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

namespace SolrNet.Mapping.Validation.Rules
{
    /// <summary>
    /// Validation rule for making sure the uniqueKey mapped in the type is the same as in the Solr schema.
    /// </summary>
    public class UniqueKeyMatchesMappingRule : IValidationRule
    {
        /// <summary>
        /// Validates that the uniqueKey mapped in the type is the same as in the Solr schema.
        /// </summary>
        /// <typeparam name="T">The type which mappings will be validated.</typeparam>
        /// <param name="solrSchema">The solr schema.</param>
        /// <param name="mappingManager">The mapping manager.</param>
        /// <returns>
        /// A collection of <see cref="MappingValidationItem"/> objects with any issues found during validation.
        /// </returns>
        public IEnumerable<MappingValidationItem> Validate<T>(SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {
            var result = new List<MappingValidationItem>();

            if (solrSchema.UniqueKey != null)
                if (!mappingManager.GetUniqueKey(typeof(T)).Value.Equals(solrSchema.UniqueKey))
                    result.Add(new MappingValidationError(
                                            String.Format("Solr schema unique key '{0}' does not match document unique key '{1}'.",
                                                          solrSchema.UniqueKey, mappingManager.GetUniqueKey(typeof(T)))));
            
            return result;
        }
    }
}
