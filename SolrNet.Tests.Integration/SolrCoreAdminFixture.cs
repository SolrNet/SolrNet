﻿using System;
using System.Configuration;
using Xunit;
using SolrNet.Commands.Cores;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;

namespace SolrNet.Tests {
    [Trait("Category", "Integration")]
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

        [Fact]
        public void GetStatusForAllCores() {
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl), GetHeaderParser(), GetStatusResponseParser());

            var results = solrCoreAdmin.Status();
            Assert.NotEmpty(results);
        }

        private HeaderResponseParser<string> GetHeaderParser() {
            return new HeaderResponseParser<string>();
        }

        private SolrStatusResponseParser GetStatusResponseParser() {
            return new SolrStatusResponseParser();
        }

        [Fact]
        public void Create() {
            var coreName = "core-new";
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            try {
                var createResponseHeader = solrCoreAdmin.Create(coreName, ".", null, null, null);
            } catch (ArgumentException) {
                // Should get an Exception here because instance directory was not specified.
                var createResponseHeader = solrCoreAdmin.Create(coreName, instanceDir);
                Assert.Equal(0, createResponseHeader.Status);
            }

            var result = solrCoreAdmin.Status(coreName);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Name);
            Assert.Equal(coreName, result.Name);
        }

        [Fact]
        public void GetStatusForNamedCore() {
            var coreName = "core-new";
            var solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl), GetHeaderParser(), GetStatusResponseParser());

            var result = solrCoreAdmin.Status(coreName);
            Assert.NotEmpty(result.Name);
            Assert.Equal(coreName, result.Name);
        }

        [Fact]
        public void ReloadCore() {
            var coreName = "core-new";
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var reloadResponseHeader = solrCoreAdmin.Reload(coreName);
            Assert.Equal(0, reloadResponseHeader.Status);
        }

        [Fact(Skip = "Our version of solr doesn't support this") ]
        public void Alias() {
            var coreName = "core-new";
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var aliasResponseHeader = solrCoreAdmin.Alias(coreName, "corefoo");
            Assert.Equal(0, aliasResponseHeader.Status);
        }

        [Fact]
        public void CreateSwapCore() {
            var coreName = "core-swap";
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var createResponseHeader = solrCoreAdmin.Create(coreName, instanceDir);
            Assert.Equal(0, createResponseHeader.Status);
            var result = solrCoreAdmin.Status(coreName);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Name);
            Assert.Equal(coreName, result.Name);
        }

        [Fact]
        public void SwapCores() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            var swapResponseHeader = solrCoreAdmin.Swap("core-new", "core-swap");
            Assert.Equal(0, swapResponseHeader.Status);
        }

        [Fact]
        public void Unload() {
            var solrCoreAdmin = new SolrCoreAdmin( new SolrConnection( solrUrl ), GetHeaderParser(), GetStatusResponseParser() );

            //var swapUnloadResponseHeader = solrCoreAdmin.Unload("core-swap", UnloadCommand.Delete.Index);
            //Assert.Equal(swapUnloadResponseHeader.Status, 0);

            var newUnloadResponseHeader = solrCoreAdmin.Unload("core-new", UnloadCommand.Delete.Index);
            Assert.Equal(0, newUnloadResponseHeader.Status);
        }
    }
}
