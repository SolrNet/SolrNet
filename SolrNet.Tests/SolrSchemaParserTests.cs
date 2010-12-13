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
using SolrNet.Schema;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests
{
    [TestFixture]
    public class SolrSchemaParserTests
    {
        [Test]
        public void SolrFieldTypeParsing()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.AreEqual(2, schemaDoc.SolrFieldTypes.Count);
            Assert.AreEqual("string", schemaDoc.SolrFieldTypes[0].Name);
            Assert.AreEqual("solr.StrField", schemaDoc.SolrFieldTypes[0].Type);
        }

        [Test]
        public void SolrFieldParsing()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.AreEqual(4, schemaDoc.SolrFields.Count);
            Assert.AreEqual("id", schemaDoc.SolrFields[0].Name);
            Assert.IsTrue(schemaDoc.SolrFields[0].IsRequired);
            Assert.IsFalse(schemaDoc.SolrFields[2].IsRequired);
            Assert.IsTrue(schemaDoc.SolrFields[3].IsMultiValued);
            Assert.IsFalse(schemaDoc.SolrFields[0].IsMultiValued);
            Assert.AreEqual("string", schemaDoc.SolrFields[0].Type.Name);
        }

        [Test]
        public void SolrDynamicFieldParsing()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.AreEqual(1, schemaDoc.SolrDynamicFields.Count);
            Assert.AreEqual("*_s", schemaDoc.SolrDynamicFields[0].Name);
        }

        [Test]
        public void SolrCopyFieldParsing()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.AreEqual(1, schemaDoc.SolrCopyFields.Count);
            Assert.AreEqual("name", schemaDoc.SolrCopyFields[0].Source);
            Assert.AreEqual("nameSort", schemaDoc.SolrCopyFields[0].Destination);
        }

        [Test]
        public void UniqueKeyPresent()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.AreEqual("id", schemaDoc.UniqueKey);
        }

        [Test]
        public void UniqueKeyNotPresent()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaNoUniqueKey.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.IsNull(schemaDoc.UniqueKey);
        }

        [Test]
        public void UniqueKeyEmpty()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaEmptyUniqueKey.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.IsNull(schemaDoc.UniqueKey);
        }

    }
}
