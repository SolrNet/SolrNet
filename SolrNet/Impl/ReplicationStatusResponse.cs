using System;

namespace SolrNet.Impl
{
    /// <summary>
    /// ReplicationStatusResponse class
    /// </summary>
	public class ReplicationStatusResponse 
    {
        /// <summary>
        /// ResponseHeader
        /// </summary>
        public ResponseHeader responseHeader { get; set; }

        /// <summary>
        /// Result status
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Result message
        /// </summary>
        public string message { get; set; }
	}
}
