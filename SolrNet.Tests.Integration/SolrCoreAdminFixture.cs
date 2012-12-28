using System;
using System.Configuration;
using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;

namespace SolrNet.Tests {
    [TestFixture]
    [Category("Integration")]
    public class SolrCoreAdminFixture {
        /*
        CREATE
        http://localhost:8983/solr/admin/cores?action=CREATE&name=core0&instanceDir=path_to_instance_directory

        RENAME
        http://localhost:8983/solr/admin/cores?action=RENAME&core=core0&other=core5

        RELOAD
        http://localhost:8983/solr/admin/cores?action=RELOAD&core=core0

        ALIAS
        http://localhost:8983/solr/admin/cores?action=ALIAS&core=core0&other=corefoo

        SWAP
        http://localhost:8983/solr/admin/cores?action=SWAP&core=core1&other=core0
         
        STATUS
        http://localhost:8983/solr/admin/cores?action=STATUS&core=core0
        
        UNLOAD
        http://localhost:8983/solr/admin/cores?action=UNLOAD&core=core0
        */

        private static readonly string solrUrl = ConfigurationManager.AppSettings["solr"];
        private const string instanceDir = "/apps/solr";

        [Test]
        public void GetStatusForAllCores() {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl), GetHeaderParser(), GetStatusResponseParser());

            var results = solrCoreAdmin.Status();
            Assert.IsNotEmpty(results);
        }

        private HeaderResponseParser<string> GetHeaderParser() {
            return new HeaderResponseParser<string>();
        }

        private SolrStatusResponseParser GetStatusResponseParser() {
            return new SolrStatusResponseParser();
        }

        [Test]
        public void Create() {
            var coreName = "core-new";
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            try {
                var createResponseHeader = solrCoreAdmin.Create(coreName, null, null, null, null);
            } catch (ArgumentException) {
                // Should get an Exception here because instance directory was not specified.
                var createResponseHeader = solrCoreAdmin.Create(coreName, instanceDir);
                Assert.AreEqual(createResponseHeader.Status, 0);
            }

            var results = solrCoreAdmin.Status(coreName);
            Assert.IsNotEmpty(results);
            Assert.IsNotEmpty(results[0].Name);
            Assert.AreEqual(coreName, results[0].Name);
        }

        [Test]
        public void GetStatusForNamedCore() {
            var coreName = "core-new";
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl), GetHeaderParser(), GetStatusResponseParser());

            var results = solrCoreAdmin.Status(coreName);
            Assert.IsNotEmpty(results[0].Name);
            Assert.AreEqual(coreName, results[0].Name);
        }

        [Test]
        public void ReloadCore() {
            var coreName = "core-new";
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var reloadResponseHeader = solrCoreAdmin.Reload(coreName);
            Assert.AreEqual(reloadResponseHeader.Status, 0);
        }

        [Test]
        public void Alias() {
            var coreName = "core-new";
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var aliasResponseHeader = solrCoreAdmin.Alias(coreName, "corefoo");
            Assert.AreEqual(aliasResponseHeader.Status, 0);
        }

        [Test]
        public void CreateSwapCore() {
            var coreName = "core-swap";
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var createResponseHeader = solrCoreAdmin.Create(coreName, instanceDir);
            Assert.AreEqual(createResponseHeader.Status, 0);
            var results = solrCoreAdmin.Status(coreName);
            Assert.IsNotEmpty(results);
            Assert.IsNotEmpty(results[0].Name);
            Assert.AreEqual(coreName, results[0].Name);
        }

        [Test]
        public void SwapCores() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var swapResponseHeader = solrCoreAdmin.Swap("core-new", "core-swap");
            Assert.AreEqual(swapResponseHeader.Status, 0);
        }

        [Test]
        public void Unload() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var swapUnloadResponseHeader = solrCoreAdmin.Unload("core-swap", true);
            Assert.AreEqual(swapUnloadResponseHeader.Status, 0);

            var newUnloadResponseHeader = solrCoreAdmin.Unload("core-new", true);
            Assert.AreEqual(newUnloadResponseHeader.Status, 0);
        }
    }
}