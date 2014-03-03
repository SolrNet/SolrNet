using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SolrNet.Commands.Replication;

namespace SolrNet.Impl 
{
    /// <summary>
    /// Solr core replication commands.
    /// </summary>
    /// <seealso href="http://wiki.apache.org/solr/SolrReplication"/>
    /// <seealso href="https://cwiki.apache.org/confluence/display/solr/Index+Replication"/>
    public class SolrCoreReplication : ISolrCoreReplication 
    {
        private readonly ISolrConnection connection;
        private readonly ISolrReplicationStatusResponseParser statusParser;
        private readonly ISolrReplicationIndexVersionResponseParser indexversionParser;
        private readonly ISolrReplicationDetailsResponseParser detailsParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrCoreReplication"/> class.
        /// </summary>
        public SolrCoreReplication(ISolrConnection connection, ISolrReplicationStatusResponseParser statusParser, ISolrReplicationIndexVersionResponseParser indexversionParser, ISolrReplicationDetailsResponseParser detailsParser)
        {
            this.connection = connection;
            this.statusParser = statusParser;
            this.indexversionParser = indexversionParser;
            this.detailsParser = detailsParser;
        }

        /// <summary>
        /// Enables replication on the master for all its slaves. 
        /// </summary>
        /// <returns></returns>
        public ReplicationStatusResponse EnableReplication()
        {
            return SendAndParseStatus(new EnableReplicationCommand());
        }

        /// <summary>
        /// Disables replication on the master for all its slaves. 
        /// </summary>
        /// <returns></returns>
        public ReplicationStatusResponse DisableReplication()
        {
            return SendAndParseStatus(new DisableReplicationCommand());
        }

        /// <summary>
        /// Returns the version of the latest replicatable index on the specified master or slave. 
        /// </summary>
        /// <returns></returns>
        public ReplicationIndexVersionResponse IndexVersion()
        {
            return SendAndParseIndexVersion(new IndexVersionCommand());
        }

        /// <summary>
        /// Retrieves configuration details and current status. 
        /// </summary>
        /// <returns></returns>
        public ReplicationDetailsResponse Details()
        {
            return SendAndParseDetails(new DetailsCommand());
        }

        /// <summary>
        /// Enables the specified slave to poll for changes on the master. 
        /// </summary>
        /// <returns></returns>
        public ReplicationStatusResponse EnablePoll()
        {
            return SendAndParseStatus(new EnablePollCommand());
        }

        /// <summary>
        /// Enables the specified slave to poll for changes on the master. 
        /// </summary>
        /// <returns></returns>
        public ReplicationStatusResponse DisablePoll()
        {
            return SendAndParseStatus(new DisablePollCommand());
        }

        /// <summary>
        /// Forces the specified slave to fetch a copy of the index from its master. If you like, you 
        /// can pass an extra attribute such as masterUrl or compression (or any other parameter which 
        /// is specified in the &lt;lst name="slave"&gt; tag) to do a one time replication from a master. 
        /// This obviates the need for hard-coding the master in the slave. 
        /// </summary>
        public ReplicationStatusResponse FetchIndex()
        {
            return FetchIndex(null);
        }

        /// <summary>
        /// Forces the specified slave to fetch a copy of the index from its master. If you like, you 
        /// can pass an extra attribute such as masterUrl or compression (or any other parameter which 
        /// is specified in the &lt;lst name="slave"&gt; tag) to do a one time replication from a master. 
        /// This obviates the need for hard-coding the master in the slave. 
        /// </summary>
        /// <param name="parameters">Optional parameters</param>
        public ReplicationStatusResponse FetchIndex(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return SendAndParseStatus(new FetchIndexCommand(parameters));
        }

        /// <summary>
        /// Aborts copying an index from a master to the specified slave.
        /// </summary>
        public ReplicationStatusResponse AbortFetch()
        {
            return SendAndParseStatus(new AbortFetchCommand());
        }

        /// <summary>
        /// Sends a command and parses the ReplicationResponse.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public ReplicationStatusResponse SendAndParseStatus(ISolrCommand cmd)
        {
            var r = Send(cmd);
            var xml = XDocument.Parse(r);
            return statusParser.Parse(xml);
        }

        /// <summary>
        /// Sends a command and parses the ReplicationResponse.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public ReplicationIndexVersionResponse SendAndParseIndexVersion(ISolrCommand cmd)
        {
            var r = Send(cmd);
            var xml = XDocument.Parse(r);
            return indexversionParser.Parse(xml);
        }

        /// <summary>
        /// Sends a command and parses the ReplicationResponse.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public ReplicationDetailsResponse SendAndParseDetails(ISolrCommand cmd)
        {
            var r = Send(cmd);
            var xml = XDocument.Parse(r);
            return detailsParser.Parse(xml);
        }

        /// <summary>
        /// Sends the specified Command to Solr.
        /// </summary>
        /// <param name="command">The Command to send.</param>
        /// <returns></returns>
        public string Send(ISolrCommand command) {
            return command.Execute(connection);
        }
    }
}
