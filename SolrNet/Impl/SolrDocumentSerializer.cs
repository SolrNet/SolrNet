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
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

        private static readonly Regex ControlCharacters =
            new Regex(@"[^\x09\x0A\x0D\x20-\uD7FF\uE000-\uFFFD\u10000-u10FFFF]", RegexOptions.Compiled);

        // http://stackoverflow.com/a/14323524/21239
        public static string RemoveControlCharacters(string xml) {
            if (xml == null)
                return null;
            return ControlCharacters.Replace(xml, "");
        }

        /// <inheritdoc />
        public XElement Serialize(T doc, double? boost) {
            var docNode = new XElement("doc");
            if (boost.HasValue) {
                var boostAttr = new XAttribute("boost", boost.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
                docNode.Add(boostAttr);
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
                    var fieldNode = new XElement("field");
                    var nameAtt = new XAttribute("name", (field.FieldName == "*" ? "" : field.FieldName) + n.FieldNameSuffix);
                    fieldNode.Add(nameAtt);

                    if (field.Boost != null && field.Boost > 0) {
                        var boostAtt = new XAttribute("boost", field.Boost.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
                        fieldNode.Add(boostAtt);
                    }

                    var v = RemoveControlCharacters(n.FieldValue);
                    if (v != null) {
                        fieldNode.Value = v;
                        docNode.Add(fieldNode);
                    }
                }
            }
            return docNode;
        }
    }
}
