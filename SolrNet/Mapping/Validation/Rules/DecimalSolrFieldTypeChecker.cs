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
using System.Linq;
using System.Text.RegularExpressions;
using SolrNet.Schema;

namespace SolrNet.Mapping.Validation.Rules {
    /// <summary>
    /// Checks schema type of properties with <see cref="decimal"/> type
    /// </summary>
    public class DecimalSolrFieldTypeChecker : ISolrFieldTypeChecker {
        /// <inheritdoc />
        public ValidationResult Validate(SolrFieldType solrFieldType, string propertyName, Type propertyType) {
            if (new[] { "solr.TextField", "solr.StrField" }.Any(st => st == solrFieldType.Type))
                return new ValidationWarning(String.Format("Property '{0}' of type '{1}' is mapped to a solr field of type '{2}'. These types are not fully compatible. You won't be able to use this field for range queries.", propertyName, propertyType.FullName, solrFieldType.Name));
            if (new[] {"FloatField", "DoubleField"}.Any(st => Regex.IsMatch(solrFieldType.Type, st)))
                return new ValidationWarning(String.Format("Property '{0}' of type '{1}' is mapped to a solr field of type '{2}'. These types are not fully compatible. You might lose precision or get OverflowExceptions", propertyName, propertyType.FullName, solrFieldType.Name));
            return new ValidationError(String.Format("Property '{0}' of type '{1}' cannot be stored in solr field type '{2}'.", propertyName, propertyType.FullName, solrFieldType.Name));
        }

        /// <inheritdoc />
        public bool CanHandleType(Type propertyType) {
            return propertyType == typeof (decimal) ||
                propertyType == typeof(decimal?);
        }
    }
}
