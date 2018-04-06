using Xunit;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests
{
    
    public class DebugResponseParserTests
    {
        [Fact]
        public void ParseDebugResponse()
        {
            var parser = new DebugResponseParser<object>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSimpleDebugDetails.xml");
            var results = new SolrQueryResults<object>();
            parser.Parse(xml, results);

            Assert.Empty(results);
            Assert.True(results.Debug is DebugResults.PlainDebugResults);
            Assert.NotNull(results.Debug.Timing);
            Assert.Equal(15, results.Debug.Timing.TotalTime);
            Assert.Equal(7, results.Debug.Timing.Prepare.Count);
            Assert.Equal(7, results.Debug.Timing.Process.Count);
        }

        [Fact]
        public void ParseResponseWithSimpleDebugExplanation()
        {
            var parser = new DebugResponseParser<object>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSimpleDebugDetails.xml");
            var results = new SolrQueryResults<object>();
            parser.Parse(xml, results);

            Assert.Empty(results);
            Assert.True(results.Debug is DebugResults.PlainDebugResults);
            Assert.NotNull(results.Debug.Explanation);
            Assert.Equal(2, results.Debug.Explanation.Count);
        }

        [Fact]
        public void ParseResponseWithStructuredDebugExplanation()
        {
            var parser = new DebugResponseParser<object>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithStructuredDebugDetails.xml");
            var results = new SolrQueryResults<object>();
            parser.Parse(xml, results);
            var debugData = results.Debug;

            Assert.Empty(results);
            Assert.NotNull(results.Debug.Explanation);
            Assert.True(results.Debug is DebugResults.StructuredDebugResults);

            var structuredDebugData = debugData as DebugResults.StructuredDebugResults;
            Assert.NotNull(structuredDebugData);
            Assert.Equal(2, structuredDebugData.ExplanationStructured.Count);
        }
    }
}
