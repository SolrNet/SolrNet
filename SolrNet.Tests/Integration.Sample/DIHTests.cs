using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests.Integration.Sample {
    [TestFixture]
    [Category("Integration")]
    public class DIHTests {
        private static readonly string serverURL = ConfigurationManager.AppSettings["solr"];

        [FixtureSetUp]
        public void FixtureSetup() {
            Console.WriteLine("FixtureSetup");
            Startup.Init<Product>(new LoggingConnection(new SolrConnection(serverURL)));
        }

        [Test]
        public void ResolveTest() {
            Assert.IsNotNull(Startup.Container.GetInstance<ISolrDIHOperations>());
        }

        [Test]
        public void BasicStatusTest() {
            var dih = Startup.Container.GetInstance<ISolrDIHOperations>();
            var status = dih.Status();
            Assert.AreEqual(DIHStatus.IDLE, status.Status);
        }
    }
}