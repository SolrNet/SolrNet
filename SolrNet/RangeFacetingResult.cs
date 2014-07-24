using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;

namespace SolrNet
{
    /// <summary>
    /// Range faceting result
    /// </summary>
    public class RangeFacetingResult
    {
        /// <summary>
        /// The range gap 
        /// </summary>
        public int Gap { get; set; }

        /// <summary>
        /// Minimum value
        /// </summary>
        public int Strart { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// The range faceting results.
        /// </summary>
        public IList<KeyValuePair<string, int>> RangeResults { get; set; }

        /// <summary>
        /// Range faceting result
        /// </summary>
        public RangeFacetingResult() {
            RangeResults = new List<KeyValuePair<string, int>>();
        }
    }
}
