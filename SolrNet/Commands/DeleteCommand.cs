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
using SolrNet.Commands.Parameters;

namespace SolrNet.Commands {
    /// <summary>
    /// Deletes document(s), either by id or by query
    /// </summary>
	public class DeleteCommand : ISolrCommand {
		private readonly ISolrDeleteParam deleteParam;

		public DeleteCommand(ISolrDeleteParam deleteParam) {
			this.deleteParam = deleteParam;
		}

        /// <summary>
        /// Deprecated in Solr 1.3
        /// </summary>
		public bool? FromPending { get; set; }

        /// <summary>
        /// Deprecated in Solr 1.3
        /// </summary>
		public bool? FromCommitted { get; set; }

		public ISolrDeleteParam DeleteParam {
			get { return deleteParam; }
		}

        private static KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

		public string Execute(ISolrConnection connection) {
			var xml = new XmlDocument();
			var deleteNode = xml.CreateElement("delete");
		    var param = new[] {
		        KV(FromPending, "fromPending"), 
                KV(FromCommitted, "fromCommitted")
		    };
		    foreach (var p in param) {
				if (p.Key.HasValue) {
					var att = xml.CreateAttribute(p.Value);
					att.InnerText = p.Key.Value.ToString().ToLower();
					deleteNode.Attributes.Append(att);
				}
			}
			deleteNode.InnerXml = string.Join("", deleteParam.ToXmlNode().Select(n => n.OuterXml).ToArray());
			return connection.Post("/update", deleteNode.OuterXml);
		}
	}
}