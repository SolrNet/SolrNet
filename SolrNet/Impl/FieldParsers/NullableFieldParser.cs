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
using System.ComponentModel;
using System.Xml;

namespace SolrNet.Impl.FieldParsers {
    public class NullableFieldParser: ISolrFieldParser {
        private readonly ISolrFieldParser parser;

        public NullableFieldParser(ISolrFieldParser parser) {
            this.parser = parser;
        }

        public bool CanHandleSolrType(string solrType) {
            return parser.CanHandleSolrType(solrType);
        }

        public Type MakeNullable(Type t) {
            return typeof (Nullable<>).MakeGenericType(t);
        }

        // From http://davidhayden.com/blog/dave/archive/2006/11/26/IsTypeNullableTypeConverter.aspx
        public Type GetUnderlyingNullableType(Type t) {
            if (!IsNullableType(t))
                return t;
            var nc = new NullableConverter(t);
            return nc.UnderlyingType;
        }

        // From http://davidhayden.com/blog/dave/archive/2006/11/26/IsTypeNullableTypeConverter.aspx
        public bool IsNullableType(Type theType) {
            return theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public bool CanHandleType(Type t) {
            return parser.CanHandleType(t) || parser.CanHandleType(GetUnderlyingNullableType(t));
        }

        public object Parse(XmlNode field, Type t) {
            if (string.IsNullOrEmpty(field.InnerText))
                return null;
            return parser.Parse(field, t);
        }
    }
}