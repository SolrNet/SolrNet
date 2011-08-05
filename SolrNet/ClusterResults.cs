using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace SolrNet
{
    //Represents an individual cluster
    [Serializable]
    public class Cluster
    {
        /// <summary>
        /// Label for the cluster
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Score of the cluster
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// List of the documents by newsItemId that are in the cluster
        /// </summary>
        public ICollection<string> Documents { get; set; }
    }

    //Represents the entire set of clusters from response
    [Serializable]
    public class ClusterResults
    {
        /// <summary>
        /// List of the clusters returned
        /// </summary>
        public ICollection<Cluster> Clusters { get; set; }

        /// <summary>
        /// Initializer
        /// </summary>
        public ClusterResults() {
            Clusters = new List<Cluster>();
        }
    }
}