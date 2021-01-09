using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;
using Moroco;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Tests.Mocks;
using Xunit.Abstractions;

namespace SolrNet.Tests {
    
    public class RegularDocumentVisitorTests {
        private readonly ITestOutputHelper testOutputHelper;

        public RegularDocumentVisitorTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InvalidCastReportsFieldName() {
            var mapper = new MReadOnlyMappingManager();
            mapper.getFields += type => {
                Assert.Equal(typeof (Entity), type);
                var model = new SolrFieldModel (
                    fieldName : "id",
                    property : typeof (Entity).GetProperty("Id"));
                return new Dictionary<string, SolrFieldModel> {
                    {"Id", model}
                };
            };
            mapper.getFields &= x => x.Expect(1);

            var parser = new MSolrFieldParser {
                canHandleSolrType = _ => true,
                canHandleType = _ => true,
                parse = (a,b) => "something",
            };

            var v = new RegularDocumentVisitor(parser, mapper);
            var doc = new Entity();
            var field = new XElement("tag");
            try {
                v.Visit(doc, "Id", field);
                Assert.True(false,"Should have failed with invalid cast");
            } catch (ArgumentException e) {
                Assert.Contains("property 'Id'", e.Message);
                testOutputHelper.WriteLine(e.Message);
            }

            mapper.getFields.Verify();
        }
    }
}
