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
using System.ComponentModel;
using System.Xml;
using SolrNet.Utils;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    /// <summary>
    /// Document visitor that handles generic dictionary properties
    /// </summary>
    public class GenericDictionaryDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly IReadOnlyMappingManager mapper;
        private readonly ISolrFieldParser parser;

        /// <summary>
        /// Document visitor that handles generic dictionary properties
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="parser"></param>
        public GenericDictionaryDocumentVisitor(IReadOnlyMappingManager mapper, ISolrFieldParser parser) {
            this.mapper = mapper;
            this.parser = parser;
        }

        /// <summary>
        /// True if this visitor can handle this Type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandleType(Type t) {
            return TypeHelper.IsGenericAssignableFrom(typeof (IDictionary<,>), t);
        }

        /// <summary>
        /// Creates a new <see cref="Dictionary{TKey,TValue}"/>
        /// </summary>
        /// <param name="typeArgs">Key and Value type parameters</param>
        /// <returns></returns>
        public object NewDictionary(Type[] typeArgs) {
            var genericType = typeof (Dictionary<,>).MakeGenericType(typeArgs);
            return Activator.CreateInstance(genericType);
        }

        /// <summary>
        /// Sets a key/value in a generic dictionary
        /// </summary>
        /// <param name="dict"><see cref="Dictionary{TKey,TValue}"/> instance</param>
        /// <param name="key">Key value</param>
        /// <param name="value">Value value</param>
        public void SetKV(object dict, object key, object value) {
            dict.GetType().GetMethod("set_Item").Invoke(dict, new[] {key, value});
        }

        public object ConvertTo(string s, Type t) {
            var converter = TypeDescriptor.GetConverter(t);
            return converter.ConvertFrom(s);
        }

        public void Visit(object doc, string fieldName, XmlNode field) {
            var allFields = mapper.GetFields(doc.GetType());
            var thisField = Func.FirstOrDefault(allFields, p => fieldName.StartsWith(p.Value) && CanHandleType(p.Key.PropertyType));
            if (thisField.Key == null)
                return;
            var thisFieldName = thisField.Value;
            if (!field.Attributes["name"].InnerText.StartsWith(thisFieldName))
                return;
            var typeArgs = thisField.Key.PropertyType.GetGenericArguments();
            var keyType = typeArgs[0];
            var valueType = typeArgs[1];
            var dict = thisField.Key.GetValue(doc, null) ?? NewDictionary(typeArgs);
            var key = field.Attributes["name"].InnerText.Substring(thisFieldName.Length);
            var value = parser.Parse(field, valueType);
            SetKV(dict, ConvertTo(key, keyType), value);
            thisField.Key.SetValue(doc, dict, null);
        }
    }
}