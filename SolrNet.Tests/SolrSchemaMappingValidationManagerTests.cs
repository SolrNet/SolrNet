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

using System.Xml;
using MbUnit.Framework;
using SolrNet.Mapping;
using SolrNet.Schema;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrSchemaMappingValidationManagerTests {
        public void RequiredSolrFieldForWhichNoCopyFieldExistsShouldReturnError()
        {
            var mgr = new MappingManager();
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("ID"), "id");
            mgr.SetUniqueKey(typeof(SchemaMappingTestDocument).GetProperty("ID"));


            var solrSchemaParser = new SolrSchemaParser();
            var schemaManager = new SolrSchemaMappingValidationManager(mgr, solrSchemaParser);

            XmlDocument schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
             
            var validationResults = schemaManager.Validate(typeof(SchemaMappingTestDocument), schemaXmlDocument);
            Assert.AreEqual(1, validationResults.Errors.Count);
        }

        [Test]
        public void MappedPropertyForWhichSolrFieldExistsInSchemaShouldNotReturnError()
        {
            var mgr = new MappingManager();
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("ID"), "id");
            mgr.SetUniqueKey(typeof(SchemaMappingTestDocument).GetProperty("ID"));
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("Name"), "name");

            var solrSchemaParser = new SolrSchemaParser();
            var schemaManager = new SolrSchemaMappingValidationManager(mgr, solrSchemaParser);

            XmlDocument schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");

            var validationResults = schemaManager.Validate(typeof(SchemaMappingTestDocument), schemaXmlDocument);
            Assert.AreEqual(0, validationResults.Errors.Count);
        }

        [Test]
        public void MappedPropertyForWhichDynamicFieldExistsInSchemaShouldNotReturnError()
        {
            var mgr = new MappingManager();
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("ID"), "id");
            mgr.SetUniqueKey(typeof(SchemaMappingTestDocument).GetProperty("ID"));
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("Name"), "name");
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("Producer"), "producer_s");

            var solrSchemaParser = new SolrSchemaParser();
            var schemaManager = new SolrSchemaMappingValidationManager(mgr, solrSchemaParser);

            XmlDocument schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");

            var validationResults = schemaManager.Validate(typeof(SchemaMappingTestDocument), schemaXmlDocument);
            Assert.AreEqual(0, validationResults.Errors.Count);
        }

        [Test]
        public void MappedPropertyForWhichNoSolrFieldOrDynamicFieldExistsShouldReturnError()
        {
            var mgr = new MappingManager();
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("ID"), "id");
            mgr.SetUniqueKey(typeof(SchemaMappingTestDocument).GetProperty("ID"));
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("Name"), "name");
            mgr.Add(typeof(SchemaMappingTestDocument).GetProperty("FieldNotSolrSchema"), "FieldNotSolrSchema");

            var solrSchemaParser = new SolrSchemaParser();
            var schemaManager = new SolrSchemaMappingValidationManager(mgr, solrSchemaParser);

            XmlDocument schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");

            var validationResults = schemaManager.Validate(typeof(SchemaMappingTestDocument), schemaXmlDocument);
            Assert.AreEqual(1, validationResults.Errors.Count);
        }             
    }

    public class SchemaMappingTestDocument
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Producer { get; set; }
        public string FieldNotSolrSchema { get; set; }
    }
}