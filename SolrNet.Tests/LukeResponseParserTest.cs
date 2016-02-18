// 
using MbUnit.Framework;
using SolrNet.Schema;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests 
{
    [TestFixture]
    public class LukeResponseParserTest 
    {

        [Test]
        public void SolrFieldTypeParsing()
        {
            var schemaParser = new LukeResponseParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.luke.xml");
            LukeResponse lukeResponse = schemaParser.Parse(xml);

            Assert.AreEqual(2, lukeResponse.SolrFields.Count);
            Assert.AreEqual("id", lukeResponse.SolrFields[0].Name);
            Assert.AreEqual("string", lukeResponse.SolrFields[0].Type.Name);
        }
    }
}