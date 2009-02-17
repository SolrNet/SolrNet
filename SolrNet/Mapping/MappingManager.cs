#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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
using System.Reflection;

namespace SolrNet.Mapping {
    /// <summary>
    /// Manual mapping manager
    /// </summary>
    public class MappingManager : IMappingManager {
        private readonly IDictionary<Type, Dictionary<PropertyInfo, string>> mappings = new Dictionary<Type, Dictionary<PropertyInfo, string>>();
        private readonly IDictionary<Type, PropertyInfo> uniqueKeys = new Dictionary<Type, PropertyInfo>();

        public void Add(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");
            Add(property, property.Name);
        }

        public void Add(PropertyInfo property, string fieldName) {
            if (property == null)
                throw new ArgumentNullException("property");
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");
            var t = property.ReflectedType;
            if (!mappings.ContainsKey(t))
                mappings[t] = new Dictionary<PropertyInfo, string>(); // new List<KeyValuePair<PropertyInfo, string>>();
            mappings[t][property] = fieldName;
        }

        /// <summary>
        /// Gets fields mapped for this type
        /// </summary>
        /// <param name="type">Document type</param>
        /// <returns>Null if <paramref name="type"/> is not mapped</returns>
        public ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");
            if (!mappings.ContainsKey(type))
                return new KeyValuePair<PropertyInfo, string>[0];
            return mappings[type];
        }

        public void SetUniqueKey(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");
            var t = property.ReflectedType;
            if (!mappings.ContainsKey(t))
                throw new ArgumentException(string.Format("Property '{0}.{1}' not mapped. Please use Add() to map it first", t, property.Name));
            uniqueKeys[t] = property;
        }

        public KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");
            var prop = uniqueKeys[type];
            return new KeyValuePair<PropertyInfo, string>(prop, mappings[type][prop]);
        }
    }
}