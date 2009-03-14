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
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.FieldParsers {
    public class CollectionFieldParser : ISolrFieldParser {
        private readonly ISolrFieldParser valueParser;

        public CollectionFieldParser(ISolrFieldParser valueParser) {
            this.valueParser = valueParser;
        }

        public bool CanHandleSolrType(string solrType) {
            return solrType == "arr";
        }

        public bool IsGenericAssignableFrom(Type t, Type other) {
            if (other.GetGenericArguments().Length != t.GetGenericArguments().Length)
                return false;
            var genericT = t.MakeGenericType(other.GetGenericArguments());
            return genericT.IsAssignableFrom(other);
        }

        public bool CanHandleType(Type t) {
            return t != typeof (string) &&
                   typeof (IEnumerable).IsAssignableFrom(t) &&
                   !typeof (IDictionary).IsAssignableFrom(t) &&
                   !IsGenericAssignableFrom(typeof (IDictionary<,>), t);
        }

        public object Parse(XmlNode field, Type t) {
            var genericTypes = t.GetGenericArguments();
            if (genericTypes.Length == 1) {
                // ICollection<int>, etc
                return GetGenericCollectionProperty(field, genericTypes);
            }
            if (t.IsArray) {
                // int[], string[], etc
                return GetArrayProperty(field, t);
            }
            if (t.IsInterface) {
                // ICollection
                return GetNonGenericCollectionProperty(field);
            }
            return null;
        }

        public IList GetNonGenericCollectionProperty(XmlNode field) {
            var l = new ArrayList();
            foreach (XmlNode arrayValueNode in field.ChildNodes) {
                l.Add(valueParser.Parse(arrayValueNode, typeof(object)));
            }
            return l;
        }


        public Array GetArrayProperty(XmlNode field, Type t) {
            // int[], string[], etc
            var arr = (Array) Activator.CreateInstance(t, new object[] {field.ChildNodes.Count});
            var arrType = Type.GetType(t.ToString().Replace("[]", ""));
            int i = 0;
            foreach (XmlNode arrayValueNode in field.ChildNodes) {
                arr.SetValue(valueParser.Parse(arrayValueNode, arrType), i);
                i++;
            }
            return arr;
        }


        public IList GetGenericCollectionProperty(XmlNode field, Type[] genericTypes) {
            // ICollection<int>, etc
            var gt = genericTypes[0];
            var l = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(gt));
            foreach (XmlNode arrayValueNode in field.ChildNodes) {
                l.Add(valueParser.Parse(arrayValueNode, gt));
            }
            return l;
        }
    }
}