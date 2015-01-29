using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet
{
    /// <summary>
    /// 
    /// </summary>
    public class RangeFacetingResult
    {
        /// <summary>
        /// Date faceting result
        /// </summary>
        public RangeFacetingResult()
        {
            RangeResults = new List<KeyValuePair<string, int>>();
            OtherResults = new Dictionary<FacetRangeOther, int>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Gap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// The range faceting results
        /// </summary>
        public IList<KeyValuePair<string, int>> RangeResults { get; set; }

        /// <summary>
        /// Other faceting results.
        /// </summary>
        public IDictionary<FacetRangeOther, int> OtherResults { get; set; }
    }
}
