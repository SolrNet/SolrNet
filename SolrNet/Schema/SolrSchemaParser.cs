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
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Exceptions;

namespace SolrNet.Schema {
    /// <summary>
    /// Parses a Solr schema xml document into a strongly typed
    /// <see cref="SolrSchema"/> object.
    /// </summary>
    public class SolrSchemaParser : ISolrSchemaParser {
        /// <summary>
        /// Parses the specified Solr schema xml.
        /// </summary>
        /// <param name="solrSchemaXml">The Solr schema xml to parse.</param>
        /// <returns>A strongly styped representation of the Solr schema xml.</returns>
        public SolrSchema Parse(XDocument solrSchemaXml) {
            var result = new SolrSchema();

            var schemaElem = solrSchemaXml.Element("schema");

            foreach (var fieldNode in schemaElem.XPathSelectElements("types/fieldType|types/fieldtype")) {
                var field = new SolrFieldType(fieldNode.Attribute("name").Value, fieldNode.Attribute("class").Value);
                result.SolrFieldTypes.Add(field);
            }

            var fieldsElem = schemaElem.Element("fields");

            foreach (var fieldNode in fieldsElem.Elements("field")) {
                var fieldTypeName = fieldNode.Attribute("type").Value;
                var fieldType = result.FindSolrFieldTypeByName(fieldTypeName);
                if (fieldType == null)
                    throw new SolrNetException(string.Format("Field type '{0}' not found", fieldTypeName));
                var field = new SolrField(fieldNode.Attribute("name").Value, fieldType);
                field.IsRequired = fieldNode.Attribute("required") != null ? fieldNode.Attribute("required").Value.ToLower().Equals(Boolean.TrueString.ToLower()) : false;
                field.IsMultiValued = fieldNode.Attribute("multiValued") != null ? fieldNode.Attribute("multiValued").Value.ToLower().Equals(Boolean.TrueString.ToLower()) : false;
                result.SolrFields.Add(field);
            }

            foreach (var dynamicFieldNode in fieldsElem.Elements("dynamicField")) {
                var dynamicField = new SolrDynamicField(dynamicFieldNode.Attribute("name").Value);
                result.SolrDynamicFields.Add(dynamicField);
            }

            foreach (var copyFieldNode in schemaElem.Elements("copyField")) {
                var copyField = new SolrCopyField(copyFieldNode.Attribute("source").Value, copyFieldNode.Attribute("dest").Value);
                result.SolrCopyFields.Add(copyField);
            }

            var uniqueKeyNode = schemaElem.Element("uniqueKey");
            if (uniqueKeyNode != null && !string.IsNullOrEmpty(uniqueKeyNode.Value)) {
                result.UniqueKey = uniqueKeyNode.Value;
            }

            return result;
        }
    }
}