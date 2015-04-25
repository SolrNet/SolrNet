using MbUnit.Framework;
using SolrNet.Impl;
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
            Assert.IsTrue(results.Debug is DebugResults.PlainDebugResults);
            Assert.IsNotNull(results.Debug.Timing);
            Assert.AreEqual(15, results.Debug.Timing.TotalTime);
            Assert.AreEqual(7, results.Debug.Timing.Prepare.Count);
            Assert.AreEqual(7, results.Debug.Timing.Process.Count);
        }

        [Test]
        public void ParseResponseWithSimpleDebugExplanation()
        {
            var parser = new DebugResponseParser<object>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSimpleDebugDetails.xml");
            var results = new SolrQueryResults<object>();
            parser.Parse(xml, results);

            Assert.AreEqual(0, results.Count);
            Assert.IsTrue(results.Debug is DebugResults.PlainDebugResults);
            Assert.IsNotNull(results.Debug.Explanation);
            Assert.AreEqual(2, results.Debug.Explanation.Count);
        }

        [Test]
        public void ParseResponseWithStructuredDebugExplanation()
        {
            var parser = new DebugResponseParser<object>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithStructuredDebugDetails.xml");
            var results = new SolrQueryResults<object>();
            parser.Parse(xml, results);
            var debugData = results.Debug;

            Assert.AreEqual(0, results.Count);
            Assert.IsNotNull(results.Debug.Explanation);
            Assert.IsTrue(results.Debug is DebugResults.StructuredDebugResults);

            var structuredDebugData = debugData as DebugResults.StructuredDebugResults;
            Assert.IsNotNull(structuredDebugData);
            Assert.AreEqual(2, structuredDebugData.ExplanationStructured.Count);
        }
    }
}
