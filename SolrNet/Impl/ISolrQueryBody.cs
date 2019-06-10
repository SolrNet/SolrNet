using System;
using System.Collections.Generic;
using System.Text;

namespace SolrNet.Impl
{
    /// <summary>
    /// Represents the body of a query to be sent to Solr.
    /// </summary>
    public interface ISolrQueryBody
    {
        /// <summary>
        /// Convert the body into a string to send to Solr.
        /// </summary>
        /// <returns></returns>
        string Serialize();

        /// <summary>
        /// The MimeType to use when sending the request.
        /// </summary>
        string MimeType { get; }
    }
}
