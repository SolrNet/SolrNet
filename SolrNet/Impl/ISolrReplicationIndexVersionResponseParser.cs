using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SolrNet.Impl
{
    /// <summary>
    /// Parses a Solr Replication result from a Replication IndexVersion command.
    /// </summary>
    public interface ISolrReplicationIndexVersionResponseParser
    {
        /// <summary>
        /// Parses the IndexVersion and Generation properyes from the response returned.
        /// </summary>
        /// <param name="xml">The XML Document to parse.</param>
        /// <returns>
        /// IndexVersion and Generation.
        /// </returns>
        ReplicationIndexVersionResponse Parse(XDocument xml);
    }
}
