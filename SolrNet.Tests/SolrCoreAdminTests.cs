namespace SolrNet.Tests
{
    #region Using Directives

    using MbUnit.Framework;
    using Rhino.Mocks;
    using SolrNet.Impl;

    #endregion

    /*
     
RENAME

http://localhost:8983/solr/admin/cores?action=RENAME&core=core0&other=core5

ALIAS

http://localhost:8983/solr/admin/cores?action=ALIAS&core=core0&other=corefoo

SWAP

http://localhost:8983/solr/admin/cores?action=SWAP&core=core1&other=core0

UNLOAD

http://localhost:8983/solr/admin/cores?action=UNLOAD&core=core0
     */

    [TestFixture]
    public class SolrCoreAdminTests
    {
        private const string solrUrl = "http://localhost:8080/solr";

        [Test]
        public void GetStatusForAllCores()
        {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl));

            solrCoreAdmin.Status();
        }

        [Test]
        public void GetStatusForNamedCore()
        {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl));

            solrCoreAdmin.Status("core0");
        }

        [Test]
        public void Create()
        {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl));

            solrCoreAdmin.Create("core-new", null, null, null, null);
        }

        [Test]
        public void ReloadCore()
        {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl));

            solrCoreAdmin.Reload("core0");
        }

        [Test]
        public void Alias()
        {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl));

            solrCoreAdmin.Alias("core0", "corefoo");
        }

        [Test]
        public void SwapCores()
        {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl));

            solrCoreAdmin.Swap("core0", "core1");
        }
        
        [Test]
        public void Unload()
        {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl));

            solrCoreAdmin.Unload("core0");
        }
    }
}