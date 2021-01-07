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
using Xunit;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;
using SolrNet.Tests.Utils;
using Xunit.Abstractions;

namespace SolrNet.Tests {
    
    public class UniqueKeyMatchesMappingRuleTests {
        private readonly ITestOutputHelper testOutputHelper;

        public UniqueKeyMatchesMappingRuleTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void NonMatchingUniqueKeyMappingShouldReturnError() {
            var mgr = new MappingManager();
            mgr.Add(typeof (SchemaMappingTestDocument).GetProperty("Name"), "name");
            mgr.SetUniqueKey(typeof (SchemaMappingTestDocument).GetProperty("Name"));

            var schemaManager = new MappingValidator(mgr, new[] {new UniqueKeyMatchesMappingRule()});

            var schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            var solrSchemaParser = new SolrSchemaParser();
            var schema = solrSchemaParser.Parse(schemaXmlDocument);

            var validationResults = schemaManager.EnumerateValidationResults(typeof (SchemaMappingTestDocument), schema).ToList();
            Assert.Single(validationResults);
        }

        [Fact]
        public void MatchingUniqueKeyMappingShouldNotReturnError() {
            var mgr = new MappingManager();
            mgr.Add(typeof (SchemaMappingTestDocument).GetProperty("ID"), "id");
            mgr.SetUniqueKey(typeof (SchemaMappingTestDocument).GetProperty("ID"));

            var schemaManager = new MappingValidator(mgr, new[] {new UniqueKeyMatchesMappingRule()});

            var schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            var solrSchemaParser = new SolrSchemaParser();
            var schema = solrSchemaParser.Parse(schemaXmlDocument);

            var validationResults = schemaManager.EnumerateValidationResults(typeof (SchemaMappingTestDocument), schema).ToList();
            Assert.Empty(validationResults);
        }

        [Fact]
        public void SchemaNull_MappingNotNull_generates_error() {
            var rule = new UniqueKeyMatchesMappingRule();
            var mapper = new MappingManager();
            var idProperty = typeof (SchemaMappingTestDocument).GetProperty("ID");
            mapper.Add(idProperty);
            mapper.SetUniqueKey(idProperty);
            var validations = rule.Validate(typeof (SchemaMappingTestDocument), new SolrSchema(), mapper).ToList();
            Assert.NotNull(validations);
            Assert.Single(validations);
            foreach (var v in validations)
                testOutputHelper.WriteLine("{0}: {1}", v.GetType(), v.Message);
            Assert.IsType<ValidationError>(validations[0]);
        }

        [Fact]
        public void SchemaNotNull_MappingNull_generates_warning() {
            var rule = new UniqueKeyMatchesMappingRule();
            var schema = new SolrSchema {UniqueKey = "id"};
            var validations = rule.Validate(typeof (SchemaMappingTestDocument), schema, new MappingManager()).ToList();
            Assert.NotNull(validations);
            Assert.Single(validations);
            foreach (var v in validations)
                testOutputHelper.WriteLine("{0}: {1}", v.GetType(), v.Message);
            Assert.IsType<ValidationWarning>(validations[0]);
        }

        [Fact]
        public void SchemaNull_MappingNull_no_errors() {
            var rule = new UniqueKeyMatchesMappingRule();
            var schema = new SolrSchema();
            var validations = rule.Validate(typeof (SchemaMappingTestDocument), schema, new MappingManager()).ToList();
            Assert.NotNull(validations);
            Assert.Empty(validations);
        }
    }
}
