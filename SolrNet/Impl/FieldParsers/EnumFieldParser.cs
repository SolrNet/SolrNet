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

namespace SolrNet.Impl.FieldParsers {
    public class EnumFieldParser : ISolrFieldParser {
        /// <inheritdoc />
        public bool CanHandleSolrType(string solrType) {
            return solrType == "str" || solrType == "int";
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return t.IsEnum;
        }

        /// <inheritdoc />
        public object Parse(XElement field, Type t) {
            if (field == null)
                throw new ArgumentNullException("field");
            if (t == null)
                throw new ArgumentNullException("t");
            var value = field.Value;
            try {
                return Enum.Parse(t, field.Value);
            } catch (Exception e) {
                throw new Exception(string.Format("Invalid value '{0}' for enum type '{1}'", value, t), e);
            }
        }
    }
}
