// 

using System.Collections.Generic;

namespace SolrNet.Schema 
{
    /// <summary>
    /// Represents a admin/luke response
    /// </summary>
    public class LukeResponse 
    {

        /// <summary>
        /// Gets or sets the solr fields.
        /// </summary>
        /// <value>The solr fields.</value>
        public List<SolrField> SolrFields { get; set; }
    }
}