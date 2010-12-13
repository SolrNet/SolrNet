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

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    /// <summary>
    /// Aggregate document visitor
    /// </summary>
    public class AggregateDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly IEnumerable<ISolrDocumentPropertyVisitor> visitors;

        /// <summary>
        /// Aggregate document visitor
        /// </summary>
        /// <param name="visitors">Visitors to aggregate</param>
        public AggregateDocumentVisitor(IEnumerable<ISolrDocumentPropertyVisitor> visitors) {
            this.visitors = visitors;
        }

        public void Visit(object doc, string fieldName, XElement field) {
            foreach (var v in visitors) {
                v.Visit(doc, fieldName, field);
            }
        }
    }
}