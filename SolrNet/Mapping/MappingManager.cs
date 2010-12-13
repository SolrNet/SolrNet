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
    /// Manual mapping manager
    /// </summary>
    public class MappingManager : IMappingManager {
        private readonly IDictionary<Type, List<SolrFieldModel>> mappings = new Dictionary<Type, List<SolrFieldModel>>();
        private readonly IDictionary<Type, PropertyInfo> uniqueKeys = new Dictionary<Type, PropertyInfo>();

        public void Add(PropertyInfo property) {
            if (property == null)
                throw new ArgumentNullException("property");
            Add(property, property.Name);
        }

        public void Add(PropertyInfo property, string fieldName)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");
            Add(property, fieldName, null);
        }

        public void Add(PropertyInfo property, string fieldName, float? boost) {
            if (property == null)
                throw new ArgumentNullException("property");
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");

            var fld = new SolrFieldModel {Property = property, FieldName = fieldName, Boost = boost};

            var t = property.ReflectedType;

            if (!mappings.ContainsKey(t))
            {
                mappings[t] = new List<SolrFieldModel>();
            }

            var idx = mappings[t].FindIndex(f => f.Property == fld.Property);

            if(idx >= 0)
            {
                mappings[t][idx] = fld;
                return;
            }

            mappings[t].Add(fld);
        }

        /// <summary>
        /// Gets fields mapped for this type
        /// </summary>
        /// <param name="type">Document type</param>
        /// <returns>Null if <paramref name="type"/> is not mapped</returns>
        public ICollection<SolrFieldModel> GetFields(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");
            if (!mappings.ContainsKey(type))
                return new SolrFieldModel[0];// KeyValuePair<PropertyInfo, string>[0];
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

        public SolrFieldModel GetUniqueKey(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");
            try {
                var prop = uniqueKeys[type];
                var unique = mappings[type].Find(t => t.Property == prop);
                return unique;
            } catch (KeyNotFoundException) {
                return null;
            }
        }

        public ICollection<Type> GetRegisteredTypes() {
            return mappings.Select(k => k.Key).Distinct().ToList();
        }
    }
}