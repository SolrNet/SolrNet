using System.Collections.Generic;

namespace SolrNet
{
    /// <summary>
    /// Contains the string values of a typed StatsResult.
    /// </summary>
    public class TypedStatsResultString : ITypedStatsResult<string>
    {
        public string Min { get; set; }
        public string Max { get; set; }
        public string Sum { get; set; }
        public string SumOfSquares { get; set; }
        public string Mean { get; set; }
        public string StdDev { get; set; }
    }
}
