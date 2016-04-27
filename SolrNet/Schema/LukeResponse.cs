using System.Collections.Generic;
using SolrNet.Schema;

namespace SolrNet.Schema 
{
    /// <summary>
    /// Represents a admin/luke response
    /// </summary>
    public class LukeResponse 
    {
        public LukeResponse()
        {
            SolrFields = new List<SolrField>();
        }

        /// <summary>
        /// Gets or sets the solr fields.
        /// </summary>
        /// <value>The solr fields.</value>
        public List<SolrField> SolrFields { get; set; }
    }
}
