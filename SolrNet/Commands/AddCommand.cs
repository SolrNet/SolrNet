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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SolrNet.Utils;
using System.Threading.Tasks;

namespace SolrNet.Commands {
	/// <summary>
	/// Adds / updates documents to solr
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class AddCommand<T> : ISolrCommand {
	    private readonly IEnumerable<KeyValuePair<T, double?>> documents = new List<KeyValuePair<T, double?>>();
	    private readonly ISolrDocumentSerializer<T> documentSerializer;
	    private readonly AddParameters parameters;

        /// <summary>
        /// Adds / updates documents to solr
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="serializer"></param>
        /// <param name="parameters"></param>
	    public AddCommand(IEnumerable<KeyValuePair<T, double?>> documents, ISolrDocumentSerializer<T> serializer, AddParameters parameters) {
            this.documents = documents;
            documentSerializer = serializer;
            this.parameters = parameters;
        }

        /// <summary>
        /// Removes UTF control characters, not valid in XML
        /// </summary>
        /// <returns></returns>
        /// <seealso href="http://cse-mjmcl.cse.bris.ac.uk/blog/2007/02/14/1171465494443.html#comment1221120563572"/>
        /// <summary>
        /// Serializes command to Solr XML
        /// </summary>
        /// <returns></returns>
        public string ConvertToXml() {
            var addElement = new XElement("add");
            if (parameters != null) {
                if (parameters.CommitWithin.HasValue) {
                    var commit = new XAttribute("commitWithin", parameters.CommitWithin.Value.ToString(CultureInfo.InvariantCulture));
                    addElement.Add(commit);
                }
                if (parameters.Overwrite.HasValue) {
                    var overwrite = new XAttribute("overwrite", parameters.Overwrite.Value.ToString().ToLowerInvariant());
                    addElement.Add(overwrite);
                }
            }
            foreach (var docWithBoost in documents) {
                var xmlDoc = documentSerializer.Serialize(docWithBoost.Key, docWithBoost.Value);
                addElement.Add(xmlDoc);
            }
            return addElement.ToString(SaveOptions.DisableFormatting);
        }

        /// <inheritdoc />
	    public string Execute(ISolrConnection connection) {
	        var xml = ConvertToXml();
			return connection.Post("/update", xml);
		}

        /// <inheritdoc />
        public Task<string> ExecuteAsync(ISolrConnection connection)
        {
            var xml = ConvertToXml();
            return connection.PostAsync("/update", xml);
        }
    }
}
