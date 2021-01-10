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
using System.ComponentModel;
using System.Xml.Linq;

namespace SolrNet.Impl.FieldParsers {
    /// <summary>
    /// Parses using <see cref="TypeConverter"/>
    /// </summary>
    public class TypeConvertingFieldParser: ISolrFieldParser {
        /// <inheritdoc />
        public bool CanHandleSolrType(string solrType) {
            return solrTypes.ContainsKey(solrType);
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return solrTypes.Values.Contains(t);
        }

        private static readonly IDictionary<string, Type> solrTypes;

        static TypeConvertingFieldParser() {
            solrTypes = new Dictionary<string, Type> {
                {"bool", typeof (bool)},
                {"str", typeof (string)},
                {"int", typeof (int)},
                {"float", typeof (float)},
                {"double", typeof(double)},
                {"long", typeof (long)},
            };
        }

        /// <summary>
        /// Gets the corresponding CLR Type to a solr type
        /// </summary>
        /// <param name="field"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Type GetUnderlyingType(XElement field, Type t) {
            if (t != typeof(object))
                return t;
            return solrTypes[field.Name.LocalName];
        }

        /// <inheritdoc />
        public object Parse(XElement field, Type t) {
            var converter = TypeDescriptor.GetConverter(GetUnderlyingType(field, t));
            if (converter != null && converter.CanConvertFrom(typeof(string)))
                return converter.ConvertFromInvariantString(field.Value);
            return Convert.ChangeType(field.Value, t);
        }
    }
}
