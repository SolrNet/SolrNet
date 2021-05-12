namespace SolrNet.Cloud {
    /// <summary>
    /// Represents cloud router
    /// </summary>
    public class SolrCloudRouter {
        /// <summary>
        /// Router name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Explicit router
        /// </summary>
        public static readonly string Explicit = "explicit";

        /// <summary>
        /// CompositeId router
        /// </summary>
        public static readonly string CompositeId = "compositeId";

        /// <summary>
        /// Constructor
        /// </summary>
        public SolrCloudRouter(string name) {
            Name = name;
        }
    }
}
