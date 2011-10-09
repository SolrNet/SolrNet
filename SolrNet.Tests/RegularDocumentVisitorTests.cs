using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;

namespace SolrNet.Tests {
    [TestFixture]
    public class RegularDocumentVisitorTests {
        [Test]
        public void InvalidCastReportsFieldName() {
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            mapper.Expect(x => x.GetFields(typeof(Entity)))
                .Return(new Dictionary<string, SolrFieldModel> {
                    {"Id", new SolrFieldModel {
                        FieldName = "id",
                        Property = typeof(Entity).GetProperty("Id"),
                    }}
                });
            var parser = MockRepository.GenerateMock<ISolrFieldParser>();
            parser.Expect(x => x.CanHandleSolrType(null))
                .IgnoreArguments()
                .Return(true);
            parser.Expect(x => x.CanHandleType(null))
                .IgnoreArguments()
                .Return(true);
            parser.Expect(x => x.Parse(null, null))
                .IgnoreArguments()
                .Return("something");
            var v = new RegularDocumentVisitor(parser, mapper);
            var doc = new Entity();
            var field = new XElement("tag");
            try {
                v.Visit(doc, "Id", field);
                Assert.Fail("Should have failed with invalid cast");
            } catch (ArgumentException e) {
                Assert.Contains(e.Message, "property 'Id'");
                Console.WriteLine(e.Message);
            }
        }
    }
}
