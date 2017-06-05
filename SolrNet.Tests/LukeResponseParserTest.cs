using System.Xml.Linq;
using MbUnit.Framework;
using SolrNet.Schema;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests 
{
    [TestFixture]
    public class LukeResponseParserTest 
    {

        private XDocument lukeResponse;
        private SolrSchema schema;



        [Test]
        public void Test_Parser()
        {
            BuildFakes();
            var parser = new LukeResponseParser();
            var response = parser.Parse(lukeResponse, schema);

            Assert.AreEqual(2, response.SolrFields.Count);
            Assert.IsNotNull(response.SolrFields.Find(x => x.Name.Equals("foo_result")));
            Assert.IsNotNull(response.SolrFields.Find(x => x.Name.Equals("bar_result")));

        }

        private void BuildFakes()
        {
            this.lukeResponse = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.luke.xml");

            FakeSchema();
        }

        private void FakeSchema()
        {
            this.schema = new SolrSchema();
            var one = new SolrFieldType("type_1", "class.1");
            var two = new SolrFieldType("type_2", "class.1");
            var field1 = new SolrField("baz", two);

            this.schema.SolrFieldTypes.Add(one);
            this.schema.SolrFieldTypes.Add(two);
            this.schema.SolrFields.Add(field1);
        }

    }
}
