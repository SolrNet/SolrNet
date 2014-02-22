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
        public ResponseHeader responseHeader { get; private set; }

        /// <summary>
        /// Result status
        /// </summary>
        public string status { get; private set; }

        /// <summary>
        /// Result message
        /// </summary>
        public string message { get; private set; }

        /// <summary>
        /// ReplicationStatusResponse constructor
        /// </summary>
        /// <param name="ResponseHeader">response header</param>
        /// <param name="Status">status</param>
        /// <param name="Message">message</param>
        public ReplicationStatusResponse(ResponseHeader ResponseHeader, string Status, string Message)
        {
            responseHeader = ResponseHeader;
            status = Status;
            message = Message;
        }
	}
}
