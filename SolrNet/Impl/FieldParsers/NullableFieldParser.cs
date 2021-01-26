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
using System.Xml.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl.FieldParsers {
    /// <summary>
    /// Wraps a <see cref="ISolrFieldParser"/> making it support the corresponding <see cref="Nullable{T}"/> type
    /// </summary>
    public class NullableFieldParser: ISolrFieldParser {
        private readonly ISolrFieldParser parser;

        public NullableFieldParser(ISolrFieldParser parser) {
            this.parser = parser;
        }

        /// <inheritdoc />
        public bool CanHandleSolrType(string solrType) {
            return parser.CanHandleSolrType(solrType);
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return parser.CanHandleType(t) || parser.CanHandleType(TypeHelper.GetUnderlyingNullableType(t));
        }

        /// <inheritdoc />
        public object Parse(XElement field, Type t) {
            if (string.IsNullOrEmpty(field.Value) && TypeHelper.IsNullableType(t))
                return null;
            return parser.Parse(field, t);
        }
    }
}
