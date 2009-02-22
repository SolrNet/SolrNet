using System;

namespace SolrNet {
    /// <summary>
    /// Random sorting of results
    /// Requires Solr 1.3+
    /// </summary>
    public class RandomSortOrder: SortOrder {
        private readonly static Random rnd = new Random();
        private const string separator = "_";

        /// <summary>
        /// Random sorting with random seed
        /// </summary>
        /// <param name="fieldName">Random sorting field as defined in schema.xml</param>
        public RandomSortOrder(string fieldName) : base(fieldName + separator + rnd.Next()) { }

        /// <summary>
        /// Random sorting with random seed, with specified order
        /// </summary>
        /// <param name="fieldName">Random sorting field as defined in schema.xml</param>
        /// <param name="order">Sort order (asc/desc)</param>
        public RandomSortOrder(string fieldName, Order order) : base(fieldName + separator + rnd.Next(), order) { }

        /// <summary>
        /// Random sorting with specified seed
        /// </summary>
        /// <param name="fieldName">Random sorting field as defined in schema.xml</param>
        /// <param name="seed">Random seed</param>
        public RandomSortOrder(string fieldName, string seed) : base(fieldName + separator + seed) { }

        /// <summary>
        /// Random sorting with specified seed, with specified order
        /// </summary>
        /// <param name="fieldName">Random sorting field as defined in schema.xml</param>
        /// <param name="seed">Random seed</param>
        /// <param name="order">Sort order (asc/desc)</param>
        public RandomSortOrder(string fieldName, string seed, Order order) : base(fieldName + separator + seed, order) { }
    }
}