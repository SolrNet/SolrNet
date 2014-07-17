using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl
{
    /// <summary>
    /// Wraps the solr response and meta data relating to the repsonse
    /// </summary>
    public class SolrQueryResponse
    {
        /// <summary>
        /// The string response recieved from solr
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Provides metadata on the solr response
        /// Getter will never return null.
        /// </summary>
        public SolrResponseMetaData MetaData { get; private set; }

        /// <summary>
        /// Response object wrapping the Solr string response and other relevant information
        /// </summary>
        /// <param name="solrResponse">The string response from Solr</param>
        /// <param name="metaData">Metadata on the Solr response</param>
        public SolrQueryResponse(string solrResponse, SolrResponseMetaData metaData = null) {
            Response = solrResponse;
            MetaData = metaData ?? new SolrResponseMetaData();
        }
    }
}
