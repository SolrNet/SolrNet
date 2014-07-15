using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl
{
    /// <summary>
    /// Wraps the solr response and meta data relating to the repsonse
    /// </summary>
    public class SolrQueryResponse : ISolrQueryResponse
    {
        /// <summary>
        /// The string response recieved from solr
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Provides metadata on the solr response
        /// </summary>
        public SolrResponseMetaData MetaData { get; set; }

        /// <summary>
        /// Response object wrapping the Solr string response and other relevant information
        /// </summary>
        /// <param name="solrResponse">The string response from Solr</param>
        public SolrQueryResponse(string solrResponse) {
            Response = solrResponse;
            MetaData  = new SolrResponseMetaData();
        }

    }
}
