using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using SolrNet.Impl;

namespace SolrNet.Tests
{
    [TestFixture]
    public class SolrCoreAdminTests
    {
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

        private const string solrUrl = "http://solrdev1:8080/solr";
        private const string instanceDir = "/apps/solr";

        [Test]
        public void GetStatusForAllCores() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ) );

            var results = solrCoreAdmin.Status();
            Assert.IsNotEmpty( results );
        }

        [Test]
        public void Create() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ) );

            try {
                solrCoreAdmin.Create( "core-new", null, null, null, null );
            }
            catch ( ArgumentException ex ) {
                // Should get an Exception here.
                solrCoreAdmin.Create( "core-new", instanceDir );
            }

            var results = solrCoreAdmin.Status( "core-new" );
            Assert.IsNotEmpty( results );
            Assert.IsNotEmpty( results[0].Name );
        }

        [Test]
        public void GetStatusForNamedCore() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ) );

            var results = solrCoreAdmin.Status( "core-new" );
            Assert.IsNotEmpty( results[0].Name );
        }

        [Test]
        public void ReloadCore() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ) );

            solrCoreAdmin.Reload( "core-new" );
        }

        [Test]
        public void Alias() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ) );

            solrCoreAdmin.Alias( "core-new", "corefoo" );
        }

        [Test]
        public void CreateSwapCore() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ) );

            solrCoreAdmin.Create( "core-swap", instanceDir );
            var results = solrCoreAdmin.Status( "core-swap" );
            Assert.IsNotEmpty( results );
            Assert.IsNotEmpty( results[0].Name );
        }

        [Test]
        public void SwapCores() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ) );

            solrCoreAdmin.Swap( "core-new", "core-swap" );
        }

        [Test]
        public void Unload() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ) );

            try {
                solrCoreAdmin.Unload( "core-swap", true );
                solrCoreAdmin.Unload( "core-new", true );
            }
            catch ( Exception coreEx ) {
            }
        }
    }
}
