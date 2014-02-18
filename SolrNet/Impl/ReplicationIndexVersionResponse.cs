using System;

namespace SolrNet.Impl {
    public class ReplicationIndexVersionResponse
    {
        /// <summary>
        /// Gets or sets the Core's Index Result.
        /// </summary>
        public ResponseHeader responseHeader { get; set; }

        /// <summary>
        /// Index number.
        /// </summary>
        public long indexversion { get; set; }

        /// <summary>
        /// Generation number.
        /// </summary>
        public long generation { get; set; }
    }
}