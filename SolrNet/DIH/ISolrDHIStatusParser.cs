using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SolrNet.Schema;

namespace SolrNet.DHI
{

    /// <summary>
    /// Provides an interface to parsing a solr schema xml document into a <see cref="SolrDHIStatus"/> object.
    /// </summary>
    public interface ISolrDHIStatusParser
    {
        /// <summary>
        /// Parses the specified solr DHI status XML.
        /// </summary>
        /// <param name="solrDHIStatusXml">The solr schema XML.</param>
        /// <returns>a object model of the solr schema.</returns>
        SolrDHIStatus Parse(XmlDocument solrDHIStatusXml);
    }
}
