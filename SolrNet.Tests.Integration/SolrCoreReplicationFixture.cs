using System.Collections.Generic;
using System.Configuration;
using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;

namespace SolrNet.Tests 
{
    [TestFixture]
    public class SolrCoreReplicationFixture 
    {
        /*

        EnableReplication
        http://localhost:8983/solr/RegisterSlave/replication?command=enablereplication

        DisableReplication
        http://localhost:8983/solr/RegisterSlave/replication?command=disablereplication
         
        IndexVersion
        http://localhost:8983/solr/RegisterSlave/replication?command=indexversion
         
        Details
        http://172.18.20.145:8983/solr/RegisterSlave/replication?command=details

        AbortFetch
        http://172.18.20.145:8983/solr/RegisterSlave/replication?command=abortfetch
         
        FetchIndex
        http://172.18.20.145:8983/solr/RegisterSlave/replication?command=fetchindex         
         
        EnablePoll
        http://172.18.20.145:8983/solr/RegisterSlave/replication?command=enablepoll
         
        DisablePoll
        http://172.18.20.145:8983/solr/RegisterSlave/replication?command=disablepoll

        */

        private static readonly string solrUrl = "http://172.18.20.145:8983/solr/Eui1"; //"http://localhost:8983/solr/collection1";
        private static readonly string solrMasterUrl = "http://localhost:8983/solr/collection1";

        [Test]
        public void ExecuteEnableReplication()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            var rh = scr.EnableReplication();

            Assert.AreEqual(rh.status, "OK");
        }

        [Test]
        public void ExecuteDisableReplication()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            var rh = scr.DisableReplication();

            Assert.AreEqual(rh.status, "OK");
        }

        [Test]
        public void ExecuteIndexVersion()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            var rh = scr.IndexVersion();

            Assert.AreEqual(rh.indexversion, 0);
            Assert.AreEqual(rh.generation, 1);
        }

        [Test]
        public void ExecuteDetails()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            var rh = scr.Details();

            Assert.AreEqual(rh.indexversion, 0);
            Assert.AreEqual(rh.generation, 1);
            Assert.AreEqual(rh.isMaster, "true");
            Assert.AreEqual(rh.isSlave, "false");
        }

        [Test]
        public void ExecuteEnablePoll()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            var rh = scr.EnablePoll();

            Assert.AreEqual(rh.status, "OK");
        }

        [Test]
        public void ExecuteDisablePoll()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            var rh = scr.DisablePoll();

            Assert.AreEqual(rh.status, "OK");
        }

        [Test]
        public void ExecuteFetchIndex()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            var rh = scr.FetchIndex();

            Assert.AreEqual(rh.status, "ERROR");
            Assert.AreEqual(rh.message, "No slave configured or no 'masterUrl' Specified");
        }

        [Test]
        public void ExecuteFetchIndexWithParameter()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            Dictionary<string, string> dParams = new Dictionary<string, string>();
            dParams.Add("masterUrl", solrMasterUrl);
            var rh = scr.FetchIndex(dParams);

            Assert.AreEqual(rh.status, "OK");
            Assert.AreEqual(rh.message, null);
        }

        [Test]
        public void ExecuteAbortFetch()
        {
            SolrCoreReplication scr = new Impl.SolrCoreReplication(new SolrConnection(solrUrl), new ReplicationStatusResponseParser<string>(), new ReplicationIndexVersionResponseParser<string>(), new ReplicationDetailsResponseParser<string>());
            var rh = scr.AbortFetch();

            Assert.AreEqual(rh.status, "ERROR");
            Assert.AreEqual(rh.message, "No slave configured");
        }
    }
}