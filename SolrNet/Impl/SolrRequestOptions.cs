using System;

namespace SolrNet.Impl
{
    /// <summary>
    /// SolrRequestOptions allow for a close control of how a request to Solr is executed.
    /// It defines explicit functionalities that can be applied to a request.
    /// </summary>
    public class SolrRequestOptions
    {
        /// <summary>
        /// Set the timeout of the request to Solr in MilliSeconds.
        /// </summary>
        public Func<int> SolrConnectionTimeout { get; set; }

    }
}
