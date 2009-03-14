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

namespace SolrNet.Impl.FieldParsers {
    public class TypeConvertingFieldParser: ISolrFieldParser {
        public bool CanHandleSolrType(string solrType) {
            return true;
        }

        public bool CanHandleType(Type t) {
            return true;
        }

        private static readonly IDictionary<string, Type> solrTypes;

        static TypeConvertingFieldParser() {
            solrTypes = new Dictionary<string, Type> {
                {"bool", typeof(bool)},
                {"str", typeof(string)},
                {"int", typeof(int)},
                {"float", typeof(float)},
            };
        }

        public Type GetUnderlyingType(XmlNode field, Type t) {
            if (t != typeof(object))
                return t;
            var solrType = field.Name;
            if (solrTypes.ContainsKey(solrType))
                return solrTypes[solrType];
            return t;
        }

        public object Parse(XmlNode field, Type t) {
            var converter = TypeDescriptor.GetConverter(GetUnderlyingType(field, t));
            if (converter != null && converter.CanConvertFrom(typeof(string)))
                return converter.ConvertFromInvariantString(field.InnerText);
            return Convert.ChangeType(field.InnerText, t);
        }
    }
}