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
using System.Xml;
using System.Xml.Linq;

namespace SolrNet.Impl.FieldParsers {
    /// <summary>
    /// Default field parser
    /// </summary>
    public class DefaultFieldParser : ISolrFieldParser {
        private readonly AggregateFieldParser parser;

        /// <summary>
        /// Default field parser
        /// </summary>
        public DefaultFieldParser() {
            parser = new AggregateFieldParser(new ISolrFieldParser[] {
                new NullableFieldParser(new IntFieldParser()),
                new NullableFieldParser(new FloatFieldParser()),
                new NullableFieldParser(new DoubleFieldParser()),
                new NullableFieldParser(new DateTimeFieldParser()),
                new NullableFieldParser(new DecimalFieldParser()),
                new NullableFieldParser(new LongFieldParser()),
                new NullableFieldParser(new EnumFieldParser()),
                new NullableFieldParser(new GuidFieldParser()),
                new CollectionFieldParser(this),
                new MoneyFieldParser(),
                new LocationFieldParser(), 
                new TypeConvertingFieldParser(),
                new InferringFieldParser(this),
            });
        }

        public bool CanHandleSolrType(string solrType) {
            return parser.CanHandleSolrType(solrType);
        }

        public bool CanHandleType(Type t) {
            return parser.CanHandleType(t);
        }

        public object Parse(XElement field, Type t) {
            return parser.Parse(field, t);
        }
    }
}