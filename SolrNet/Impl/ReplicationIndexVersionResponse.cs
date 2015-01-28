using System;

namespace SolrNet.Impl 
{
    /// <summary>
    /// ReplicationIndexVersionResponse class
    /// </summary>
    public class ReplicationIndexVersionResponse
    {
        /// <summary>
        /// Gets or sets the Core's Index Result.
        /// </summary>
        public ResponseHeader responseHeader { get; private set; }

        /// <summary>
        /// Index number.
        /// </summary>
        public long indexversion { get; private set; }

        /// <summary>
        /// Generation number.
        /// </summary>
        public long generation { get; private set; }

        /// <summary>
        /// ReplicationIndexVersionResponse constructor
        /// </summary>
        /// <param name="ResponseHeader">response header</param>
        /// <param name="IndexVersion">index version</param>
        /// <param name="Generation">generation</param>
        public ReplicationIndexVersionResponse(ResponseHeader ResponseHeader, long IndexVersion, long Generation)
        {
            responseHeader = ResponseHeader;
            indexversion = IndexVersion;
            generation = Generation;
        }
    }
}
