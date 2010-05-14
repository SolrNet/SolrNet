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
using System.Xml;
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
        public SolrSchema Parse(XmlDocument solrSchemaXml) {
            var result = new SolrSchema();

            foreach (XmlNode fieldNode in solrSchemaXml.SelectNodes("/schema/types/fieldType|/schema/types/fieldtype")) {
                var field = new SolrFieldType(fieldNode.Attributes["name"].Value, fieldNode.Attributes["class"].Value);
                result.SolrFieldTypes.Add(field);
            }

            foreach (XmlNode fieldNode in solrSchemaXml.SelectNodes("/schema/fields/field")) {
                var fieldTypeName = fieldNode.Attributes["type"].Value;
                var fieldType = result.FindSolrFieldTypeByName(fieldTypeName);
                if (fieldType == null)
                    throw new SolrNetException(string.Format("Field type '{0}' not found", fieldTypeName));
                var field = new SolrField(fieldNode.Attributes["name"].Value, fieldType);
                field.IsRequired = fieldNode.Attributes["required"] != null ? fieldNode.Attributes["required"].Value.ToLower().Equals(Boolean.TrueString.ToLower()) : false;
                field.IsMultiValued = fieldNode.Attributes["multiValued"] != null ? fieldNode.Attributes["multiValued"].Value.ToLower().Equals(Boolean.TrueString.ToLower()) : false;
                result.SolrFields.Add(field);
            }

            foreach (XmlNode dynamicFieldNode in solrSchemaXml.SelectNodes("/schema/fields/dynamicField")) {
                var dynamicField = new SolrDynamicField(dynamicFieldNode.Attributes["name"].Value);
                result.SolrDynamicFields.Add(dynamicField);
            }

            foreach (XmlNode copyFieldNode in solrSchemaXml.SelectNodes("/schema/copyField")) {
                var copyField = new SolrCopyField(copyFieldNode.Attributes["source"].Value, copyFieldNode.Attributes["dest"].Value);
                result.SolrCopyFields.Add(copyField);
            }

            var uniqueKeyNode = solrSchemaXml.SelectSingleNode("/schema/uniqueKey");
            if (uniqueKeyNode != null && !string.IsNullOrEmpty(uniqueKeyNode.InnerText)) {
                result.UniqueKey = uniqueKeyNode.InnerText;
            }

            return result;
        }
    }
}