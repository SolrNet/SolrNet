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
using System.Text;
using System.Xml;

namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// Parameter to delete document(s) by one or more ids
    /// and or a query parameters.
    /// </summary>
    public class DeleteByIdAndOrQueryParam : ISolrDeleteParam {
        private readonly IEnumerable<string> ids;
        private readonly ISolrQuery query;

        public DeleteByIdAndOrQueryParam(IEnumerable<string> ids, ISolrQuery q) {
            this.ids = ids;
            query = q;
        }

        public IEnumerable<XmlNode> ToXmlNode() {
            var xml = new XmlDocument();
            if (ids != null)
            {
                foreach (var i in ids)
                {
                    var node = xml.CreateElement("id");
                    node.InnerText = i;
                    yield return node;
                }
            }
            if (query != null)
            {
                var queryNode = xml.CreateElement("query");
                //queryNode.InnerText = query.Query; TODO
                throw new NotImplementedException();
                yield return queryNode;
            }
        }
    }
}
