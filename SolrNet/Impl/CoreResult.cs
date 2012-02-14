using System;

namespace SolrNet.Impl {
    public class CoreResult {
        /// <summary>
        /// The name of the Core.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The directory the Solr instance is located at.
        /// </summary>
        public string InstanceDir { get; set; }

        /// <summary>
        /// The directory all data for this Core is located at.
        /// </summary>
        public string DataDir { get; set; }

        /// <summary>
        /// The time when this Core was last started.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The number of milliseconds this Core has been live.
        /// </summary>
        public long Uptime { get; set; }

        /// <summary>
        /// Gets or sets the Core's Index Result.
        /// </summary>
        public CoreIndexResult Index { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreResult"/> class.
        /// </summary>
        public CoreResult() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreResult"/> class.
        /// </summary>
        /// <param name="coreName">Name of the core.</param>
        public CoreResult(string coreName) {
            Name = coreName;
        }
    }
}