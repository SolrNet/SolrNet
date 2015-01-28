using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SolrNet.Impl
{
    /// <summary>
    /// Parses a Solr Replication result from a Replication Details command.
    /// </summary>
    public interface ISolrReplicationDetailsResponseParser
    {
        /// <summary>
        /// Parses the Details properyes from the response returned.
        /// </summary>
        /// <param name="xml">The XML Document to parse.</param>
        /// <returns>
        /// Details.
        /// </returns>
        ReplicationDetailsResponse Parse(XDocument xml);
    }
}
