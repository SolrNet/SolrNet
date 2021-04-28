using System.Collections.Generic;

namespace SolrNet {
    /// <summary>
    /// A Single group of documents
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Group<T> {
        /// <summary>
        /// The groupvalue for this group of documents
        /// </summary>
        public string GroupValue { get; set; }

        /// <summary>
        /// Returns the number of matching documents that are found for this groupValue
        /// </summary>
        public long NumFound { get; set; }

        /// <summary>
        /// The actual documents in the group.
        /// You can control the amount of documents in this collection by using the Limit property of the GroupingParameters
        /// </summary>
        public ICollection<T> Documents { get; set; }

        /// <summary>
        /// A single group of documents
        /// </summary>
        public Group() {
            Documents = new List<T>();
        }
    }
}
