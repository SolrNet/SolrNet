using System.Linq;
using System.Collections.Generic;
using Xunit;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests 
{
    
    public class SolrCoreReplicationTests 
    {
        /*

        EnableReplication
        http://localhost:8983/solr/techproducts/collection1/replication?command=enablereplication

        DisableReplication
        http://localhost:8983/solr/techproducts/collection1/replication?command=disablereplication
         
        IndexVersion
        http://localhost:8983/solr/techproducts/collection1/replication?command=indexversion
         
        Details
        http://localhost:8983/solr/techproducts/collection1/replication?command=details

        AbortFetch
        http://localhost:8983/solr/techproducts/collection1/replication?command=abortfetch
         
        FetchIndex
        http://localhost:8983/solr/techproducts/collection1/replication?command=fetchindex         
         
        EnablePoll
        http://localhost:8983/solr/techproducts/collection1/replication?command=enablepoll
         
        DisablePoll
        http://localhost:8983/solr/techproducts/collection1/replication?command=disablepoll

        */

        [Fact]
        public void ReplicationStatusOk()
        {
            var parser = new ReplicationStatusResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationStatusOk.xml");
            var results = parser.Parse(xml);

            Assert.NotNull(results.responseHeader);
            Assert.Equal("OK", results.status);
            Assert.Null(results.message);         
        }

        [Fact]
        public void ReplicationStatusError()
        {
            var parser = new ReplicationStatusResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationStatusError.xml");
            var results = parser.Parse(xml);

            Assert.NotNull(results.responseHeader);
            Assert.Equal("ERROR", results.status);
            Assert.NotNull(results.message);
            Assert.Equal("No slave configured or no 'masterUrl' Specified", results.message);
        }

        [Fact]
        public void ReplicationIndexVersion()
        {
            var parser = new ReplicationIndexVersionResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationIndexVersion.xml");
            var results = parser.Parse(xml);

            Assert.NotNull(results.responseHeader);
            Assert.Equal(0, results.indexversion);
            Assert.Equal(1, results.generation);
        }

        [Fact]
        public void ReplicationDetailsMaster()
        {
            var parser = new ReplicationDetailsResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationDetailsMaster.xml");
            var results = parser.Parse(xml);

            Assert.NotNull(results.responseHeader);
            Assert.Equal(1391678354779, results.indexVersion);
            Assert.Equal(3821, results.generation);
            Assert.Equal("914.55 MB", results.indexSize);
            Assert.Equal("/usr/share/solr/solr-4.5.0/example/solr/Eui1/data/index/", results.indexPath);
            Assert.Equal("true", results.isMaster);
            Assert.Equal("false", results.isSlave);
            Assert.Null(results.isReplicating);            
            Assert.Null(results.timeRemaining);
            Assert.Null(results.totalPercent);
        }

        [Fact]
        public void ReplicationDetailsSlaveNotReplicating()
        {
            var parser = new ReplicationDetailsResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationDetailsSlaveNotReplicating.xml");
            var results = parser.Parse(xml);

            Assert.NotNull(results.responseHeader);
            Assert.Equal(1391591222457, results.indexVersion);
            Assert.Equal(3820, results.generation);
            Assert.Equal("94 bytes", results.indexSize);
            Assert.Equal("/usr/share/solr/solr-4.5.0/example/solr/Eui1/data/index/", results.indexPath);
            Assert.Equal("false", results.isMaster);
            Assert.Equal("true", results.isSlave);
            Assert.Equal("false", results.isReplicating);
            Assert.Null(results.timeRemaining);
            Assert.Null(results.totalPercent);
        }

        [Fact]
        public void ReplicationDetailsSlaveIsReplicating()
        {
            var parser = new ReplicationDetailsResponseParser<string>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseReplicationDetailsSlaveIsReplicating.xml");
            var results = parser.Parse(xml);

            Assert.NotNull(results.responseHeader);
            Assert.Equal(1391591222457, results.indexVersion);
            Assert.Equal(3820, results.generation);
            Assert.Equal("94 bytes", results.indexSize);
            Assert.Equal("/usr/share/solr/solr-4.5.0/example/solr/Eui1/data/index/", results.indexPath);
            Assert.Equal("false", results.isMaster);
            Assert.Equal("true", results.isSlave);
            Assert.Equal("true", results.isReplicating);
            Assert.Equal("8s", results.timeRemaining);
            Assert.Equal("37.0", results.totalPercent);
        }
    }
}
