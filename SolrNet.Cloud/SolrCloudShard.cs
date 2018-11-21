using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    /// <summary>
    /// Represents cloud shard
    /// </summary>
    public class SolrCloudShard {
        /// <summary>
        /// Is active
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Shard name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Shard range end
        /// </summary>
        public int? RangeEnd { get; private set; }

        /// <summary>
        /// Shard range start
        /// </summary>
        public int? RangeStart { get; private set; }

        /// <summary>
        /// Shard replicas
        /// </summary>
        public IDictionary<string, SolrCloudReplica> Replicas { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SolrCloudShard(bool isActive, string name, int? rangeEnd, int? rangeStart, IDictionary<string, SolrCloudReplica> replicas) {
            if (replicas == null)
                throw new ArgumentNullException("replicas");
            IsActive = isActive;
            Name = name;
            RangeEnd = rangeEnd;
            RangeStart = rangeStart;
            Replicas = replicas;
        }
    }
}
