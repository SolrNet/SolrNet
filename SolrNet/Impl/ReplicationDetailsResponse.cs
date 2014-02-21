using System;

namespace SolrNet.Impl 
{
    /// <summary>
    /// ReplicationDetailsResponse class
    /// </summary>
    public class ReplicationDetailsResponse
    {
        /// <summary>
        /// Gets or sets the Core's Index Result.
        /// </summary>
        public ResponseHeader responseHeader { get; set; }

        /// <summary>
        /// Index size.
        /// </summary>
        public string indexSize { get; set; }

        /// <summary>
        /// Index path.
        /// </summary>
        public string indexPath { get; set; }

        /// <summary>
        /// Is Master.
        /// </summary>
        public string isMaster { get; set; }

        /// <summary>
        /// Is Slave.
        /// </summary>
        public string isSlave { get; set; }

        /// <summary>
        /// Index number.
        /// </summary>
        public long indexversion { get; set; }

        /// <summary>
        /// Generation number.
        /// </summary>
        public long generation { get; set; }

        /// <summary>
        /// Is replicating.
        /// </summary>
        public string isReplicating { get; set; }

        /// <summary>
        /// Total percent.
        /// </summary>
        public string totalPercent { get; set; }

        /// <summary>
        /// Time remaining.
        /// </summary>
        public string timeRemaining { get; set; }
    }
}
