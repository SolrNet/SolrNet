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
using System.Linq;
using MbUnit.Framework;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class UniqueKeyMatchesMappingRuleTests {
        [Test]
        public void NonMatchingUniqueKeyMappingShouldReturnError() {
            var mgr = new MappingManager();
            mgr.Add(typeof (SchemaMappingTestDocument).GetProperty("Name"), "name");
            mgr.SetUniqueKey(typeof (SchemaMappingTestDocument).GetProperty("Name"));

            var schemaManager = new MappingValidator(mgr, new[] {new UniqueKeyMatchesMappingRule()});

            var schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            var solrSchemaParser = new SolrSchemaParser();
            var schema = solrSchemaParser.Parse(schemaXmlDocument);

            var validationResults = schemaManager.EnumerateValidationResults(typeof (SchemaMappingTestDocument), schema).ToList();
            Assert.AreEqual(1, validationResults.Count);
        }

        [Test]
        public void MatchingUniqueKeyMappingShouldNotReturnError() {
            var mgr = new MappingManager();
            mgr.Add(typeof (SchemaMappingTestDocument).GetProperty("ID"), "id");
            mgr.SetUniqueKey(typeof (SchemaMappingTestDocument).GetProperty("ID"));

            var schemaManager = new MappingValidator(mgr, new[] {new UniqueKeyMatchesMappingRule()});

            var schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            var solrSchemaParser = new SolrSchemaParser();
            var schema = solrSchemaParser.Parse(schemaXmlDocument);

            var validationResults = schemaManager.EnumerateValidationResults(typeof (SchemaMappingTestDocument), schema).ToList();
            Assert.AreEqual(0, validationResults.Count);
        }

        [Test]
        public void SchemaNull_MappingNotNull_generates_error() {
            var rule = new UniqueKeyMatchesMappingRule();
            var mapper = new MappingManager();
            var idProperty = typeof (SchemaMappingTestDocument).GetProperty("ID");
            mapper.Add(idProperty);
            mapper.SetUniqueKey(idProperty);
            var validations = rule.Validate(typeof (SchemaMappingTestDocument), new SolrSchema(), mapper).ToList();
            Assert.IsNotNull(validations);
            Assert.AreEqual(1, validations.Count);
            foreach (var v in validations)
                Console.WriteLine("{0}: {1}", v.GetType(), v.Message);
            Assert.IsInstanceOfType<ValidationError>(validations[0]);
        }

        [Test]
        public void SchemaNotNull_MappingNull_generates_warning() {
            var rule = new UniqueKeyMatchesMappingRule();
            var schema = new SolrSchema {UniqueKey = "id"};
            var validations = rule.Validate(typeof (SchemaMappingTestDocument), schema, new MappingManager()).ToList();
            Assert.IsNotNull(validations);
            Assert.AreEqual(1, validations.Count);
            foreach (var v in validations)
                Console.WriteLine("{0}: {1}", v.GetType(), v.Message);
            Assert.IsInstanceOfType<ValidationWarning>(validations[0]);
        }

        [Test]
        public void SchemaNull_MappingNull_no_errors() {
            var rule = new UniqueKeyMatchesMappingRule();
            var schema = new SolrSchema();
            var validations = rule.Validate(typeof (SchemaMappingTestDocument), schema, new MappingManager()).ToList();
            Assert.IsNotNull(validations);
            Assert.AreEqual(0, validations.Count);
        }
    }
}