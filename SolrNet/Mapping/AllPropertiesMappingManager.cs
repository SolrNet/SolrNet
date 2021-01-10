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
using System.Linq;
using System.Reflection;

namespace SolrNet.Mapping {
    /// <summary>
    /// Maps all properties in the class, with the same field name as the property.
    /// Note that unique keys must be added manually.
    /// </summary>
    public class AllPropertiesMappingManager : IReadOnlyMappingManager {
        private readonly IDictionary<Type, PropertyInfo> uniqueKeys = new Dictionary<Type, PropertyInfo>();

        /// <inheritdoc />
        public IDictionary<string,SolrFieldModel> GetFields(Type type) {
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var fldProps = props
                .Select(prop => new SolrFieldModel(prop, prop.Name, null))
                .Select(m => new KeyValuePair<string, SolrFieldModel>(m.FieldName, m))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            return fldProps;
        }

        /// <inheritdoc />
        public SolrFieldModel GetUniqueKey(Type type) {
            try {
                var propertyInfo = uniqueKeys[type];
	            return new SolrFieldModel(propertyInfo, propertyInfo.Name, null);
            } catch (KeyNotFoundException) {
                return null;
            }
        }

        /// <inheritdoc />
        public ICollection<Type> GetRegisteredTypes() {
            return new List<Type>();
        }

        /// <summary>
        /// Sets the property that acts as unique key for a document type
        /// </summary>
        /// <param name="property">Unique key property</param>
        public void SetUniqueKey(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");
            var t = property.ReflectedType;
            uniqueKeys[t] = property;
        }
    }
}
