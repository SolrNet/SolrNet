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
using System.Globalization;
using System.Xml;

namespace SolrNet.Impl {
    /// <summary>
    /// Serializes a dictionary document
    /// </summary>
    public class SolrDictionarySerializer : ISolrDocumentSerializer<Dictionary<string, object>> {
        private readonly ISolrFieldSerializer serializer;

        public SolrDictionarySerializer(ISolrFieldSerializer serializer) {
            this.serializer = serializer;
        }

        public XmlDocument Serialize(Dictionary<string, object> doc, double? boost) {
            if (doc == null)
                throw new ArgumentNullException("doc");
            var xml = new XmlDocument();
            var docNode = xml.CreateElement("doc");
            if (boost.HasValue) {
                var boostAttr = xml.CreateAttribute("boost");
                boostAttr.Value = boost.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
                docNode.Attributes.Append(boostAttr);
            }
            foreach (var kv in doc) {
                var nodes = serializer.Serialize(kv.Value);
                foreach (var n in nodes) {
                    var name = xml.CreateAttribute("name");
                    name.Value = kv.Key;
                    var fieldNode = xml.CreateElement("field");
                    fieldNode.Attributes.Append(name);
                    fieldNode.InnerText = n.FieldValue;
                    docNode.AppendChild(fieldNode);
                }
            }
            xml.AppendChild(docNode);
            return xml;
        }
    }
}