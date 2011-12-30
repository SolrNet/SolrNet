﻿#region license
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
using SolrNet.Exceptions;

namespace SolrNet.Mapping {
    /// <summary>
    /// Manual mapping manager
    /// </summary>
    public class MappingManager : IMappingManager {
        private readonly IDictionary<Type, Dictionary<string,SolrFieldModel>> mappings = new Dictionary<Type, Dictionary<string,SolrFieldModel>>();
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

            if (!mappings.ContainsKey(t)) {
                mappings[t] = new Dictionary<string,SolrFieldModel>();
            }

            EnsurePropertyNotAlreadyAddedOnInheritedType(property, t);

            var m = mappings[t].FirstOrDefault(k => k.Value.Property == property);
            if (m.Key != null) {
                mappings[t].Remove(m.Key);
            }

            mappings[t][fieldName] = fld;
        }

        /// <summary>
        /// Gets fields mapped for this type
        /// </summary>
        /// <param name="type">Document type</param>
        /// <returns>Null if <paramref name="type"/> is not mapped</returns>
        public IDictionary<string,SolrFieldModel> GetFields(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");
            
            return mappings.Keys.Where(t => t.IsAssignableFrom(type))
                .SelectMany(t => mappings[t]).ToDictionary(pair => pair.Key, pair => pair.Value);
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
                var prop = uniqueKeys.Where(k => k.Key.IsAssignableFrom(type)).Single().Value;

                return FindPropertyInMappings(prop, type);
            } catch (KeyNotFoundException) {
                return null;
            } catch (InvalidOperationException) {
                return null;
            }
        }

        public ICollection<Type> GetRegisteredTypes() {
            return mappings.Select(k => k.Key).ToList();
        }

        private void EnsurePropertyNotAlreadyAddedOnInheritedType(PropertyInfo property, Type t)
        {
            foreach (var dic in mappings)
            {
                if (!dic.Key.IsAssignableFrom(t) && !t.IsAssignableFrom(dic.Key))
                    continue;

                if (dic.Key == t)
                    continue;

                if (dic.Value.Any(k => k.Value.Property.Name == property.Name))
                    throw new SolrNetException(string.Format("There is no need to add the same property twice for inherited types ('{0}' and'{1}' are part of the same class hierarchy, and property '{2}' was added twice).",
                                                             t, dic.Key, property.Name));
            }
        }

        private SolrFieldModel FindPropertyInMappings(PropertyInfo prop, Type type)
        {
            foreach (var dic in mappings)
            {
                if (!dic.Key.IsAssignableFrom(type) && !type.IsAssignableFrom(dic.Key))
                    continue;

                if (dic.Value.Any(k => k.Value.Property == prop))
                    return dic.Value.First(kv => kv.Value.Property == prop).Value;
            }

            throw new KeyNotFoundException(string.Format("No '{0}' property added for class '{1}'.", prop.Name, type));
        }
    }
}