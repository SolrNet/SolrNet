namespace SolrNet.Cloud {
    /// <summary>
    /// Represents cloud router
    /// </summary>
    public class SolrCloudRouter {
        /// <summary>
        /// Router name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Explicit router
        /// </summary>
        public static string Explicit = "explicit";

        /// <summary>
        /// CompositeId router
        /// </summary>
        public static string CompositId = "compositeId";

        /// <summary>
        /// Constructor
        /// </summary>
        public SolrCloudRouter(string name) {
            Name = name;
        }
    }
}
