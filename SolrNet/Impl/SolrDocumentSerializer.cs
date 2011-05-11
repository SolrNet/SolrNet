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
using System.Globalization;
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

        public XmlDocument Serialize(T doc, double? boost) {
            var xml = new XmlDocument();
            var docNode = xml.CreateElement("doc");
            if (boost.HasValue) {
                var boostAttr = xml.CreateAttribute("boost");
                boostAttr.Value = boost.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
                docNode.Attributes.Append(boostAttr);
            }
            var fields = mappingManager.GetFields(doc.GetType());
            foreach (var field in fields.Values) {
                var p = field.Property;
                if (!p.CanRead)
                    continue;
                var value = p.GetValue(doc, null);
                if (value == null)
                    continue;
                var nodes = fieldSerializer.Serialize(value);
                foreach (var n in nodes) {
                    var fieldNode = xml.CreateElement("field");
                    var nameAtt = xml.CreateAttribute("name");
                    nameAtt.InnerText = (field.FieldName == "*" ? "" : field.FieldName) + n.FieldNameSuffix;
                    fieldNode.Attributes.Append(nameAtt);

                    if (field.Boost != null && field.Boost > 0) {
                        var boostAtt = xml.CreateAttribute("boost");
                        boostAtt.InnerText = field.Boost.ToString();
                        fieldNode.Attributes.Append(boostAtt);
                    }

                    fieldNode.InnerText = n.FieldValue;
                    docNode.AppendChild(fieldNode);
                }
            }
            xml.AppendChild(docNode);
            return xml;            
        }
    }
}