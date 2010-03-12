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
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;

namespace SolrNet.Schema {
    public class MappingTypesAreCompatibleWithSolrTypesRule : IValidationRule {
        private readonly IDictionary<Type, ISolrFieldTypeChecker> fieldTypeCheckers;

        public MappingTypesAreCompatibleWithSolrTypesRule(IDictionary<Type, ISolrFieldTypeChecker> fieldTypeCheckers) {
            this.fieldTypeCheckers = fieldTypeCheckers;
        }

        #region IValidationRule Members

        public IEnumerable<MappingValidationItem> Validate<T>(SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {
            foreach (var x in mappingManager.GetFields(typeof (T)))
                if (fieldTypeCheckers.ContainsKey(x.Key.PropertyType)) {
                    MappingValidationItem i = fieldTypeCheckers[x.Key.PropertyType].Validate(solrSchema.FindSolrFieldByName(x.Value).Type, x.Key.Name, typeof(T));
                    if (i != null)
                        yield return i;
                }
        }

        #endregion
    }
}