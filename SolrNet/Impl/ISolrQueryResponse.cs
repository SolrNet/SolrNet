using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl
{
    /// <summary>
    /// Definition of response of a Solr Query
    /// </summary>
    public interface ISolrQueryResponse
    {
        /// <summary>
        /// The string repsonse recieved from solr
        /// </summary>
        string Response { get; }

        /// <summary>
        /// Provides metadata on the solr response
        /// </summary>
        SolrResponseMetaData MetaData { get; set; }
    }
}
