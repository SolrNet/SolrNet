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
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SolrNet.Impl.FieldParsers {
    /// <summary>
    /// Parser that infers .net type based on solr type
    /// </summary>
    public class InferringFieldParser : ISolrFieldParser {
        private readonly ISolrFieldParser parser;

        public InferringFieldParser(ISolrFieldParser parser) {
            this.parser = parser;
        }

        /// <inheritdoc />
        public bool CanHandleSolrType(string solrType) {
            return true;
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return true;
        }

        private static readonly IDictionary<string, Type> solrTypes;

        static InferringFieldParser() {
            solrTypes = new Dictionary<string, Type> {
                {"bool", typeof (bool)},
                {"str", typeof (string)},
                {"int", typeof (int)},
                {"float", typeof (float)},
                {"double", typeof(double)},
                {"long", typeof (long)},
                {"arr", typeof (ICollection)},
                {"date", typeof (DateTime)},
            };
        }

        /// <inheritdoc />
        public object Parse(XElement field, Type t) {
            var type = solrTypes[field.Name.LocalName];
            return parser.Parse(field, type);
        }
    }
}
