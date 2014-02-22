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
        public ResponseHeader responseHeader { get; private set; }

        /// <summary>
        /// Index size.
        /// </summary>
        public string indexSize { get; private set; }

        /// <summary>
        /// Index path.
        /// </summary>
        public string indexPath { get; private set; }

        /// <summary>
        /// Is Master.
        /// </summary>
        public string isMaster { get; private set; }

        /// <summary>
        /// Is Slave.
        /// </summary>
        public string isSlave { get; private set; }

        /// <summary>
        /// Index number.
        /// </summary>
        public long indexVersion { get; private set; }

        /// <summary>
        /// Generation number.
        /// </summary>
        public long generation { get; private set; }

        /// <summary>
        /// Is replicating.
        /// </summary>
        public string isReplicating { get; private set; }

        /// <summary>
        /// Total percent.
        /// </summary>
        public string totalPercent { get; private set; }

        /// <summary>
        /// Time remaining.
        /// </summary>
        public string timeRemaining { get; private set; }

        /// <summary>
        /// ReplicationDetailsResponse constructor
        /// </summary>
        /// <param name="ResponseHeader"></param>
        /// <param name="IndexSize"></param>
        /// <param name="IndexPath"></param>
        /// <param name="IsMaster"></param>
        /// <param name="IsSlave"></param>
        /// <param name="IndexVersion"></param>
        /// <param name="Generation"></param>
        /// <param name="IsReplicating"></param>
        /// <param name="TotalPercent"></param>
        /// <param name="TimeRemaining"></param>
        public ReplicationDetailsResponse(ResponseHeader ResponseHeader, string IndexSize, string IndexPath, string IsMaster, string IsSlave, long IndexVersion, long Generation, string IsReplicating, string TotalPercent, string TimeRemaining)
        {
            responseHeader = ResponseHeader;
            indexSize = IndexSize;
            indexPath = IndexPath;
            isMaster = IsMaster;
            isSlave = IsSlave;
            indexVersion = IndexVersion;
            generation = Generation;
            isReplicating = IsReplicating;
            totalPercent = TotalPercent;
            timeRemaining = TimeRemaining;
        }

    }
}
