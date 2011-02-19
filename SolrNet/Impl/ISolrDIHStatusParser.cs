using System.Xml.Linq;

namespace SolrNet.Impl {
    /// <summary>
    /// Provides an interface to parsing a solr schema xml document into a <see cref="SolrDIHStatus"/> object.
    /// </summary>
    public interface ISolrDIHStatusParser {
        /// <summary>
        /// Parses the specified solr DIH status XML.
        /// </summary>
        /// <param name="solrDIHStatusXml">The solr schema XML.</param>
        /// <returns>a object model of the solr schema.</returns>
        SolrDIHStatus Parse(XDocument solrDIHStatusXml);
    }
}
