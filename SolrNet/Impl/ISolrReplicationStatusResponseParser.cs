using System.Xml.Linq;

namespace SolrNet.Impl 
{
    /// <summary>
    /// Parses a Solr Replication result from a Replication Status command.
    /// </summary>
    public interface ISolrReplicationStatusResponseParser 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml">The XML Document to parse.</param>
        /// <returns>
        /// Status.
        /// </returns>
        ReplicationStatusResponse Parse(XDocument xml);
    }
}
