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
    public class MappingTypesAreCompatibleWithSolrTypesRule : IValidationRule {
        private readonly ISolrFieldTypeChecker[] fieldTypeCheckers;

        public MappingTypesAreCompatibleWithSolrTypesRule(ISolrFieldTypeChecker[] fieldTypeCheckers) {
            this.fieldTypeCheckers = fieldTypeCheckers;
        }

        public IEnumerable<ValidationResult> Validate(Type documentType, SolrSchema solrSchema, IReadOnlyMappingManager mappingManager) {
            foreach (var x in mappingManager.GetFields(documentType)) {
                var solrField = solrSchema.FindSolrFieldByName(x.FieldName);
                if (solrField == null)
                    continue;
                foreach (var checker in fieldTypeCheckers) {
                    if (!checker.CanHandleType(x.Property.PropertyType))
                        continue;
                    var i = checker.Validate(solrField.Type, x.Property.Name, x.Property.PropertyType);
                    if (i != null)
                        yield return i;
                }
            }
        }
    }
}