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

using System.Xml;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Parameter to delete one document, by id
    /// </summary>
	public class DeleteByIdParam : ISolrDeleteParam {
		private readonly string id;

		public DeleteByIdParam(string id) {
			this.id = id;
		}

		public XmlNode ToXmlNode() {
			var xml = new XmlDocument();
			var idNode = xml.CreateElement("id");
			idNode.InnerText = id;
			return idNode;
		}
	}
}