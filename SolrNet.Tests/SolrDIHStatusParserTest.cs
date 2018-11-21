using System;
using SolrNet.Impl;
using SolrNet.Tests.Utils;
using Xunit;

namespace SolrNet.Tests
{
    
    public class SolrDIHStatusParserTest {
        [Fact]
        public void SolrDIHStatusParsing() {
            var DIHStatusParser = new SolrDIHStatusParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrDIHStatusBasic.xml");
            SolrDIHStatus dihStatusDoc = DIHStatusParser.Parse(xml);

            Assert.Equal(1, dihStatusDoc.TotalRequestToDataSource);
            Assert.Equal(DateTime.Parse("2010-12-09 00:00:00"), dihStatusDoc.Committed);
            Assert.Equal(DateTime.Parse("2010-12-09 00:00:00"), dihStatusDoc.FullDumpStarted);
            Assert.Equal("", dihStatusDoc.ImportResponse);
            Assert.Equal(DateTime.Parse("2010-12-09 00:00:00"), dihStatusDoc.Optimized);
            Assert.Equal(DIHStatus.IDLE, dihStatusDoc.Status);
            Assert.Equal("Indexing completed. Added/Updated: 764648 documents. Deleted 0 documents.", dihStatusDoc.Summary.Trim());
            Assert.Equal(new TimeSpan(), dihStatusDoc.TimeElapsed);
            Assert.Equal(new TimeSpan(0, 0, 9, 48, 65), dihStatusDoc.TimeTaken);
            Assert.Equal(735352, dihStatusDoc.TotalDocumentsFailed);
            Assert.Equal(764648, dihStatusDoc.TotalDocumentsProcessed);
            Assert.Equal(0, dihStatusDoc.TotalDocumentsSkipped);
            Assert.Equal(1500000, dihStatusDoc.TotalRowsFetched);
        }

    }
}