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

using System;
using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl {
    /// <summary>
    /// Serializes a Solr document to xml
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrDocumentSerializer<T> : ISolrDocumentSerializer<T> {
        private readonly IReadOnlyMappingManager mappingManager;
        private readonly ISolrFieldSerializer fieldSerializer;

        public SolrDocumentSerializer(IReadOnlyMappingManager mappingManager, ISolrFieldSerializer fieldSerializer) {
            this.mappingManager = mappingManager;
            this.fieldSerializer = fieldSerializer;
        }

        /// <summary>
        /// Serializes a Solr document to xml
        /// </summary>
        /// <param name="doc">document to serialize</param>
        /// <returns>serialized document</returns>
        public XmlDocument Serialize(T doc) {
            var xml = new XmlDocument();
            var docNode = xml.CreateElement("doc");
            var fields = mappingManager.GetFields(typeof (T));
            foreach (var kv in fields) {
                var p = kv.Key;
                var value = p.GetValue(doc, null);
                if (value == null)
                    continue;
                var nodes = fieldSerializer.Serialize(value);
                foreach (var n in nodes) {
                    var fieldNode = xml.CreateElement("field");
                    var nameAtt = xml.CreateAttribute("name");
                    nameAtt.InnerText = kv.Value + n.FieldNameSuffix;
                    fieldNode.Attributes.Append(nameAtt);
                    fieldNode.InnerText = n.FieldValue;
                    docNode.AppendChild(fieldNode);
                }
            }
            xml.AppendChild(docNode);
            return xml;
        }

        private static readonly IDictionary<Type, string> solrTypes;

        static SolrDocumentSerializer() {
            solrTypes = new Dictionary<Type, string>();
            solrTypes[typeof (int)] = "int";
            solrTypes[typeof (string)] = "str";
            solrTypes[typeof (bool)] = "bool";
            solrTypes[typeof (DateTime)] = "date";
        }
    }
}