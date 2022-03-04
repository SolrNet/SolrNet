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

using Xunit;
using SolrNet.Schema;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests
{
    
    public class SolrSchemaParserTests
    {

#region "schema.xml"
        [Fact]
        public void SolrFieldTypeParsing()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.Equal(2, schemaDoc.SolrFieldTypes.Count);
            Assert.Equal("string", schemaDoc.SolrFieldTypes[0].Name);
            Assert.Equal("solr.StrField", schemaDoc.SolrFieldTypes[0].Type);
        }

        [Fact]
        public void SolrFieldParsing()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.Equal(4, schemaDoc.SolrFields.Count);
            Assert.Equal("id", schemaDoc.SolrFields[0].Name);
            
            Assert.True(schemaDoc.SolrFields[0].IsRequired);
            Assert.False(schemaDoc.SolrFields[2].IsRequired);

            Assert.True(schemaDoc.SolrFields[3].IsMultiValued);
            Assert.False(schemaDoc.SolrFields[0].IsMultiValued);

            Assert.True(schemaDoc.SolrFields[2].IsIndexed);
            Assert.False(schemaDoc.SolrFields[3].IsIndexed);

            Assert.True(schemaDoc.SolrFields[0].IsStored);
            Assert.False(schemaDoc.SolrFields[3].IsStored);

            Assert.False(schemaDoc.SolrFields[1].IsDocValues);
            Assert.False(schemaDoc.SolrFields[2].IsDocValues);
            Assert.True(schemaDoc.SolrFields[3].IsDocValues);

            Assert.Equal("string", schemaDoc.SolrFields[0].Type.Name);
        }

        [Fact]
        public void SolrDynamicFieldParsing()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.Single(schemaDoc.SolrDynamicFields);
            Assert.Equal("*_s", schemaDoc.SolrDynamicFields[0].Name);
            Assert.False(schemaDoc.SolrDynamicFields[0].IsRequired);
            Assert.False(schemaDoc.SolrDynamicFields[0].IsMultiValued);
            Assert.True(schemaDoc.SolrDynamicFields[0].IsIndexed);
            Assert.True(schemaDoc.SolrDynamicFields[0].IsStored);
            Assert.False(schemaDoc.SolrDynamicFields[0].IsDocValues);
            Assert.Equal("string", schemaDoc.SolrDynamicFields[0].Type.Name);
        }

        [Fact]
        public void SolrCopyFieldParsing()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.Single(schemaDoc.SolrCopyFields);
            Assert.Equal("name", schemaDoc.SolrCopyFields[0].Source);
            Assert.Equal("nameSort", schemaDoc.SolrCopyFields[0].Destination);
        }

        [Fact]
        public void UniqueKeyPresent()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.Equal("id", schemaDoc.UniqueKey);
        }

        [Fact]
        public void UniqueKeyNotPresent()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaNoUniqueKey.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.Null(schemaDoc.UniqueKey);
        }

        [Fact]
        public void UniqueKeyEmpty()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaEmptyUniqueKey.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.Null(schemaDoc.UniqueKey);
        }
        #endregion


#region "managed-schema"
        [Fact]
        public void SolrFieldTypeParsingManagedSchema()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrManagedSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.Equal(2, schemaDoc.SolrFieldTypes.Count);
            Assert.Equal("string", schemaDoc.SolrFieldTypes[0].Name);
            Assert.Equal("solr.StrField", schemaDoc.SolrFieldTypes[0].Type);
        }

        [Fact]
        public void SolrFieldParsingManagedSchema()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrManagedSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.Equal(4, schemaDoc.SolrFields.Count);
            Assert.Equal("id", schemaDoc.SolrFields[0].Name);

            Assert.True(schemaDoc.SolrFields[0].IsRequired);
            Assert.False(schemaDoc.SolrFields[2].IsRequired);

            Assert.True(schemaDoc.SolrFields[3].IsMultiValued);
            Assert.False(schemaDoc.SolrFields[0].IsMultiValued);

            Assert.True(schemaDoc.SolrFields[2].IsIndexed);
            Assert.False(schemaDoc.SolrFields[3].IsIndexed);

            Assert.True(schemaDoc.SolrFields[0].IsStored);
            Assert.False(schemaDoc.SolrFields[3].IsStored);

            Assert.False(schemaDoc.SolrFields[1].IsDocValues);
            Assert.False(schemaDoc.SolrFields[2].IsDocValues);
            Assert.True(schemaDoc.SolrFields[3].IsDocValues);

            Assert.Equal("string", schemaDoc.SolrFields[0].Type.Name);
        }

        [Fact]
        public void SolrDynamicFieldParsingManagedSchema()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrManagedSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.Single(schemaDoc.SolrDynamicFields);
            Assert.Equal("*_s", schemaDoc.SolrDynamicFields[0].Name);
            Assert.False(schemaDoc.SolrDynamicFields[0].IsRequired);
            Assert.False(schemaDoc.SolrDynamicFields[0].IsMultiValued);
            Assert.True(schemaDoc.SolrDynamicFields[0].IsIndexed);
            Assert.True(schemaDoc.SolrDynamicFields[0].IsStored);
            Assert.False(schemaDoc.SolrDynamicFields[0].IsDocValues);
            Assert.Equal("string", schemaDoc.SolrDynamicFields[0].Type.Name);
        }

        [Fact]
        public void SolrCopyFieldParsingManagedSchema()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrManagedSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);

            Assert.Single(schemaDoc.SolrCopyFields);
            Assert.Equal("name", schemaDoc.SolrCopyFields[0].Source);
            Assert.Equal("nameSort", schemaDoc.SolrCopyFields[0].Destination);
        }

        [Fact]
        public void UniqueKeyPresentManagedSchema()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrManagedSchemaBasic.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.Equal("id", schemaDoc.UniqueKey);
        }

        [Fact]
        public void UniqueKeyNotPresentManagedSchema()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrManagedSchemaNoUniqueKey.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.Null(schemaDoc.UniqueKey);
        }

        [Fact]
        public void UniqueKeyEmptyManagedSchema()
        {
            var schemaParser = new SolrSchemaParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrManagedSchemaEmptyUniqueKey.xml");
            SolrSchema schemaDoc = schemaParser.Parse(xml);
            Assert.Null(schemaDoc.UniqueKey);
        }
        #endregion

    }
}
