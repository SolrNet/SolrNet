using System;
using System.Collections.Generic;
using System.Text;

namespace SolrNet.Cloud.Exceptions
{
    /// <summary>
    /// Error connecting to SolrNet Cloud. See inner exception for more information.
    /// </summary>
    public class SolrNetCloudConnectionException : ApplicationException
    {
        public SolrNetCloudConnectionException(string message) : base(message)
        {
        }

        public SolrNetCloudConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
