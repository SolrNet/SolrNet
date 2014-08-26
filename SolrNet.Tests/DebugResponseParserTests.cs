using System.Linq;
using MbUnit.Framework;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests
{
    [TestFixture]
    public class DebugResponseParserTests
    {
        [Test]
        public void ParseDebugResponse()
        {
            var parser = new DebugResponseParser<object>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSimpleDebugDetails.xml");
            var results = new SolrQueryResults<object>();
            parser.Parse(xml, results);

            Assert.AreEqual(0, results.Count);
            Assert.IsNotNull(results.Debug.Timing);
            Assert.AreEqual(15, results.Debug.Timing.TotalTime);
            Assert.AreEqual(7, results.Debug.Timing.Prepare.Count);
            Assert.AreEqual(7, results.Debug.Timing.Process.Count);
        }

        [Test]
        public void ParseResponseWithSimpleDebugExplaination()
        {
            var parser = new DebugResponseParser<object>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSimpleDebugDetails.xml");
            var results = new SolrQueryResults<object>();
            parser.Parse(xml, results);

            Assert.AreEqual(0, results.Count);
            Assert.IsNotNull(results.Debug.Explain);
            Assert.IsNotNull(results.Debug.ExplainStructured);
            Assert.AreEqual(2, results.Debug.Explain.Count);
            Assert.AreEqual(0, results.Debug.ExplainStructured.Count());
        }

        [Test]
        public void ParseResponseWithStructuredDebugExplaination()
        {
            var parser = new DebugResponseParser<object>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithStructuredDebugDetails.xml");
            var results = new SolrQueryResults<object>();
            parser.Parse(xml, results);

            Assert.AreEqual(0, results.Count);
            Assert.IsNotNull(results.Debug.Explain);
            Assert.IsNotNull(results.Debug.ExplainStructured);
            Assert.AreEqual(0, results.Debug.Explain.Count);
            Assert.AreEqual(2, results.Debug.ExplainStructured.Count());
        }
    }
}
