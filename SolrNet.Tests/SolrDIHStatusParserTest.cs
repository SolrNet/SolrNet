using System;
using SolrNet.Impl;
using SolrNet.Tests.Utils;
using MbUnit.Framework;

namespace SolrNet.Tests
{
    [TestFixture]
    public class SolrDIHStatusParserTest {
        [Test]
        public void SolrDIHStatusParsing() {
            var DIHStatusParser = new SolrDIHStatusParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrDIHStatusBasic.xml");
            SolrDIHStatus dihStatusDoc = DIHStatusParser.Parse(xml);

            Assert.AreEqual(1, dihStatusDoc.TotalRequestToDataSource);
            Assert.AreEqual(DateTime.Parse("2010-12-09 00:00:00"), dihStatusDoc.Committed);
            Assert.AreEqual(DateTime.Parse("2010-12-09 00:00:00"), dihStatusDoc.FullDumpStarted);
            Assert.AreEqual("", dihStatusDoc.ImportResponse);
            Assert.AreEqual(DateTime.Parse("2010-12-09 00:00:00"), dihStatusDoc.Optimized);
            Assert.AreEqual(DIHStatus.IDLE, dihStatusDoc.Status);
            Assert.AreEqual("Indexing completed. Added/Updated: 764648 documents. Deleted 0 documents.", dihStatusDoc.Summary.Trim());
            Assert.AreEqual(new TimeSpan(), dihStatusDoc.TimeElapsed);
            Assert.AreEqual(new TimeSpan(0, 0, 9, 48, 65), dihStatusDoc.TimeTaken);
            Assert.AreEqual(735352, dihStatusDoc.TotalDocumentsFailed);
            Assert.AreEqual(764648, dihStatusDoc.TotalDocumentsProcessed);
            Assert.AreEqual(0, dihStatusDoc.TotalDocumentsSkipped);
            Assert.AreEqual(1500000, dihStatusDoc.TotalRowsFetched);
        }

    }
}