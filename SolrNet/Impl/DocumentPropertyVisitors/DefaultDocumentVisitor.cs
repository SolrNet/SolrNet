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

using System.Xml;
using System.Xml.Linq;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    /// <summary>
    /// Default document visitor
    /// </summary>
    public class DefaultDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly AggregateDocumentVisitor visitor;

        /// <summary>
        /// Default document visitor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="parser"></param>
        public DefaultDocumentVisitor(IReadOnlyMappingManager mapper, ISolrFieldParser parser) {
            visitor = new AggregateDocumentVisitor(new ISolrDocumentPropertyVisitor[] {
                new RegularDocumentVisitor(parser, mapper),
                new GenericDictionaryDocumentVisitor(mapper, parser),
            });
        }

        public void Visit(object doc, string fieldName, XElement field) {
            visitor.Visit(doc, fieldName, field);
        }
    }
}