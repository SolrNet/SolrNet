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
        public string Name { get; private set; }

        /// <summary>
        /// Collection router type
        /// </summary>
        public SolrCloudRouter Router { get; set; }

        /// <summary>
        /// Collection shards
        /// </summary>
        public IDictionary<string, SolrCloudShard> Shards { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SolrCloudCollection(string name, SolrCloudRouter router, IDictionary<string, SolrCloudShard> shards) {
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
