using System.Linq;
using System.Collections.Generic;
using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests 
{
    [TestFixture]
    public class SolrCoreReplicationTests 
    {
        /*

        EnableReplication
        http://localhost:8983/solr/collection1/replication?command=enablereplication

        DisableReplication
        http://localhost:8983/solr/collection1/replication?command=disablereplication
         
        IndexVersion
        http://localhost:8983/solr/collection1/replication?command=indexversion
         
        Details
        http://localhost:8983/solr/collection1/replication?command=details

        AbortFetch
        http://localhost:8983/solr/collection1/replication?command=abortfetch
         
        FetchIndex
        http://localhost:8983/solr/collection1/replication?command=fetchindex         
         
        EnablePoll
        http://localhost:8983/solr/collection1/replication?command=enablepoll
         
        DisablePoll
        http://localhost:8983/solr/collection1/replication?command=disablepoll

        */

        [Test]
        public void ReplicationStatusOk()
        {
            var parser = new ReplicationStatusResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationStatusOk.xml");
            var results = parser.Parse(xml);

            Assert.IsNotNull(results.responseHeader);
            Assert.AreEqual("OK", results.status);
            Assert.IsNull(results.message);         
        }

        [Test]
        public void ReplicationStatusError()
        {
            var parser = new ReplicationStatusResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationStatusError.xml");
            var results = parser.Parse(xml);

            Assert.IsNotNull(results.responseHeader);
            Assert.AreEqual("ERROR", results.status);
            Assert.IsNotNull(results.message);
            Assert.AreEqual("No slave configured or no 'masterUrl' Specified", results.message);
        }

        [Test]
        public void ReplicationIndexVersion()
        {
            var parser = new ReplicationIndexVersionResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationIndexVersion.xml");
            var results = parser.Parse(xml);

            Assert.IsNotNull(results.responseHeader);
            Assert.AreEqual(0, results.indexversion);
            Assert.AreEqual(1, results.generation);
        }

        [Test]
        public void ReplicationDetailsMaster()
        {
            var parser = new ReplicationDetailsResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationDetailsMaster.xml");
            var results = parser.Parse(xml);

            Assert.IsNotNull(results.responseHeader);
            Assert.AreEqual(1391678354779, results.indexVersion);
            Assert.AreEqual(3821, results.generation);
            Assert.AreEqual("914.55 MB", results.indexSize);
            Assert.AreEqual("/usr/share/solr/solr-4.5.0/example/solr/Eui1/data/index/", results.indexPath);
            Assert.AreEqual("true", results.isMaster);
            Assert.AreEqual("false", results.isSlave);
            Assert.IsNull(results.isReplicating);            
            Assert.IsNull(results.timeRemaining);
            Assert.IsNull(results.totalPercent);
        }

        [Test]
        public void ReplicationDetailsSlaveNotReplicating()
        {
            var parser = new ReplicationDetailsResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationDetailsSlaveNotReplicating.xml");
            var results = parser.Parse(xml);

            Assert.IsNotNull(results.responseHeader);
            Assert.AreEqual(1391591222457, results.indexVersion);
            Assert.AreEqual(3820, results.generation);
            Assert.AreEqual("94 bytes", results.indexSize);
            Assert.AreEqual("/usr/share/solr/solr-4.5.0/example/solr/Eui1/data/index/", results.indexPath);
            Assert.AreEqual("false", results.isMaster);
            Assert.AreEqual("true", results.isSlave);
            Assert.AreEqual("false", results.isReplicating);
            Assert.IsNull(results.timeRemaining);
            Assert.IsNull(results.totalPercent);
        }

        [Test]
        public void ReplicationDetailsSlaveIsReplicating()
        {
            var parser = new ReplicationDetailsResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationDetailsSlaveIsReplicating.xml");
            var results = parser.Parse(xml);

            Assert.IsNotNull(results.responseHeader);
            Assert.AreEqual(1391591222457, results.indexVersion);
            Assert.AreEqual(3820, results.generation);
            Assert.AreEqual("94 bytes", results.indexSize);
            Assert.AreEqual("/usr/share/solr/solr-4.5.0/example/solr/Eui1/data/index/", results.indexPath);
            Assert.AreEqual("false", results.isMaster);
            Assert.AreEqual("true", results.isSlave);
            Assert.AreEqual("true", results.isReplicating);
            Assert.AreEqual("8s", results.timeRemaining);
            Assert.AreEqual("37.0", results.totalPercent);
        }
    }
}
