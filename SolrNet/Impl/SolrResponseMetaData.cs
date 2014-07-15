using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl
{
    /// <summary>
    /// Supplies metadata on a solr response
    /// </summary>
    public class SolrResponseMetaData
    {
        /// <summary>
        /// The original query issued by Solr to get the response
        /// </summary>
        public string OriginalQuery { get; set; }
    }
}
