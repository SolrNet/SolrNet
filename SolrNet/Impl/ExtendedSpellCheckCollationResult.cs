using System.Collections.Generic;

namespace SolrNet.Impl {
    /// <summary>
    /// Spellcheck collation results
    /// </summary>
    public class ExtendedSpellCheckCollationResult {
        /// <summary>
        /// The new collated query
        /// </summary>
        public string CollationQuery { get; set; }

        /// <summary>
        /// Number of hits that the current collation might return
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// A summary of each correction made
        /// </summary>
        public ICollection<KeyValuePair<string, string>> MisspellingsAndCorrections { get; set; }
    }
}