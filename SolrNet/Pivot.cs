using System.Collections.Generic;

namespace SolrNet {
    /// <summary>
    /// Pivot facet
    /// </summary>
    public class Pivot {
        /// <summary>
        /// Pivot field name
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Pivot value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Pivot facet count
        /// </summary>
        public int Count { get; set; }

        public List<Pivot> ChildPivots { get; set; }

        public bool HasChildPivots { get; set; }

        /// <summary>
        /// Pivot facet
        /// </summary>
        public Pivot() {
            HasChildPivots = false;
        }
    }
}