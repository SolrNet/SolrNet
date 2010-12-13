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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl.FieldParsers {
    /// <summary>
    /// Aggregates <see cref="ISolrFieldParser"/>s
    /// </summary>
    public class AggregateFieldParser : ISolrFieldParser {
        private readonly IEnumerable<ISolrFieldParser> parsers;

        /// <summary>
        /// Aggregates <see cref="ISolrFieldParser"/>s
        /// </summary>
        /// <param name="parsers"></param>
        public AggregateFieldParser(IEnumerable<ISolrFieldParser> parsers) {
            this.parsers = parsers;
        }

        public bool CanHandleSolrType(string solrType) {
            return parsers.Any(p => p.CanHandleSolrType(solrType));
        }

        public bool CanHandleType(Type t) {
            return parsers.Any(p => p.CanHandleType(t));
        }

        public object Parse(XElement field, Type t) {
            return parsers
                .Where(p => p.CanHandleType(t) && p.CanHandleSolrType(field.Name.LocalName))
                .Select(p => p.Parse(field, t))
                .FirstOrDefault();
        }
    }
}