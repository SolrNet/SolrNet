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
using System.Reflection;
using SolrNet.Attributes;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet.Mapping {
    /// <summary>
    /// Gets mapping info from attributes like <see cref="SolrFieldAttribute"/> and <see cref="SolrUniqueKeyAttribute"/>
    /// </summary>
    public class AttributesMappingManager : IReadOnlyMappingManager {
        public IEnumerable<KeyValuePair<PropertyInfo, T[]>> GetPropertiesWithAttribute<T>(Type type) where T : Attribute {
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var kvAttrs = Func.Select(props, prop => new KeyValuePair<PropertyInfo, T[]>(prop, GetCustomAttributes<T>(prop)));
            var propsAttrs = Func.Filter(kvAttrs, kv => kv.Value.Length > 0);
            return propsAttrs;
        }

        public ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type) {
            var propsAttrs = GetPropertiesWithAttribute<SolrFieldAttribute>(type);
            var fields = Func.Select(propsAttrs, kv => new KeyValuePair<PropertyInfo, string>(kv.Key, kv.Value[0].FieldName ?? kv.Key.Name));
            return new List<KeyValuePair<PropertyInfo, string>>(fields);
        }

        public T[] GetCustomAttributes<T>(PropertyInfo prop) where T : Attribute {
            return (T[]) prop.GetCustomAttributes(typeof (T), true);
        }

        public KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type) {
            var propsAttrs = GetPropertiesWithAttribute<SolrUniqueKeyAttribute>(type);
            var fields = Func.Select(propsAttrs, kv => new KeyValuePair<PropertyInfo, string>(kv.Key, kv.Value[0].FieldName ?? kv.Key.Name));
            try {
                return Func.First(fields);
            } catch (InvalidOperationException) {
                throw new NoUniqueKeyException(type);
            }
        }

        public ICollection<Type> GetRegisteredTypes() {
            var types = new List<Type>();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
                try {
                    foreach (var t in a.GetTypes()) {
                        if (GetFields(t).Count > 0)
                            types.Add(t);
                    }
                } catch (ReflectionTypeLoadException) {
                    // if I can't get an assembly's types, just ignore it
                }
            }
            return types;
        }
    }
}