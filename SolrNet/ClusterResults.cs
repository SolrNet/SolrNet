using System.Collections.Generic;
using System;

namespace SolrNet {
    /// <summary>
    /// Document cluster
    /// </summary>
    [Serializable]
    public class Cluster {
        /// <summary>
        /// Cluster label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Cluster score
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// List of the documents by id that are in the cluster
        /// </summary>
        public ICollection<string> Documents { get; set; }
    }

    /// <summary>
    /// Cluster results
    /// </summary>
    [Serializable]
    public class ClusterResults {
        /// <summary>
        /// List of the clusters returned
        /// </summary>
        public ICollection<Cluster> Clusters { get; set; }

        /// <summary>
        /// Cluster results
        /// </summary>
        public ClusterResults() {
            Clusters = new List<Cluster>();
        }
    }
}