using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    /// <summary>
    /// Represents cloud collection
    /// </summary>
    public class SolrCloudCollection {
        /// <summary>
        /// Collection name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Collection router type
        /// </summary>
        public SolrCloudRouter Router { get; }

        /// <summary>
        /// Collection shards
        /// </summary>
        public IReadOnlyDictionary<string, SolrCloudShard> Shards { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SolrCloudCollection(string name, SolrCloudRouter router, IReadOnlyDictionary<string, SolrCloudShard> shards) {
            if (router == null)
                throw new ArgumentNullException("router");
            if (shards == null)
                throw new ArgumentNullException("shards");
            Name = name;
            Router = router;
            Shards = shards;
        }
    }
}
