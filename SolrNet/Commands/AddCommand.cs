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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace SolrNet.Commands {
	/// <summary>
	/// Adds / updates documents to solr
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class AddCommand<T> : ISolrCommand {
		private readonly IEnumerable<T> documents = new List<T>();
	    private readonly ISolrDocumentSerializer<T> documentSerializer;

	    /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="documents">Documents to add</param>
		/// <param name="serializer">document serializer</param>
		public AddCommand(IEnumerable<T> documents, ISolrDocumentSerializer<T> serializer) {
	        this.documents = documents;
	        documentSerializer = serializer;
		}

        /// <summary>
        /// Removes UTF control characters, not valid in XML
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        /// <see cref="http://cse-mjmcl.cse.bris.ac.uk/blog/2007/02/14/1171465494443.html#comment1221120563572"/>
        public string RemoveControlCharacters(string xml) {
            return Regex.Replace(xml, "&\\#x(0?[0-8BCEF]|1[0-9A-F]|FFF[E-F]);", "");
        }

        public string ConvertToXml() {
            var xml = new XmlDocument();
            var addElement = xml.CreateElement("add");
            foreach (T doc in documents) {
                var xmlDoc = documentSerializer.Serialize(doc);
                addElement.AppendChild(xml.ImportNode(xmlDoc.DocumentElement, true));
            }
            xml.AppendChild(addElement);
            return xml.OuterXml;
        }

	    public string Execute(ISolrConnection connection) {
	        var xml = ConvertToXml();
	        xml = RemoveControlCharacters(xml);
			return connection.Post("/update", xml);
		}
	}
}