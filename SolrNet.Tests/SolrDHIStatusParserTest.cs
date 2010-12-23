using System;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet.DHI;
using SolrNet.Tests.Utils;
using Assert = MbUnit.Framework.Assert;

namespace SolrNet.Tests
{
    [TestClass]
    public class SolrDHIStatusParserTest
    {
        [TestMethod]
        public void SolrDHIStatusParing()
        {
            var DHIStatusParser = new SolrDHIStatusParser();
            XmlDocument xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrDHIStatusBasic.xml");
            SolrDHIStatus dhiStatusDoc = DHIStatusParser.Parse(xml);

            Assert.AreEqual(1, dhiStatusDoc.TotalRequestToDataSource);
            Assert.AreEqual(DateTime.Parse("2010-12-09 00:00:00"), dhiStatusDoc.Committed);
            Assert.AreEqual(DateTime.Parse("2010-12-09 00:00:00"), dhiStatusDoc.FullDumpStarted);
            Assert.AreEqual("", dhiStatusDoc.ImportResponse);
            Assert.AreEqual(DateTime.Parse("2010-12-09 00:00:00"), dhiStatusDoc.Optimized);
            Assert.AreEqual(DHIStatus.IDLE, dhiStatusDoc.Status);
            Assert.AreEqual("\r\n      Indexing completed. Added/Updated: 764648 documents. Deleted 0 documents.\r\n    ", dhiStatusDoc.Summary);
            Assert.AreEqual(new TimeSpan(), dhiStatusDoc.TimeElapsed);
            Assert.AreEqual(new TimeSpan(0, 0, 9, 48, 65), dhiStatusDoc.TimeTaken);
            Assert.AreEqual(735352, dhiStatusDoc.TotalDocumentsFailed);
            Assert.AreEqual(764648, dhiStatusDoc.TotalDocumentsProcessed);
            Assert.AreEqual(0, dhiStatusDoc.TotalDocumentsSkipped);
            Assert.AreEqual(1500000, dhiStatusDoc.TotalRowsFetched);
        }

    }
}