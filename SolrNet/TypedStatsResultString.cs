using System.Collections.Generic;

namespace SolrNet
{
    /// <summary>
    /// Contains the string values of a `TypedStatsResult`.
    /// </summary>
    public class TypedStatsResultString : TypedStatsResult<string>
    {
        public override string Min { get; set; }
        public override string Max { get; set; }
        public override string Sum { get; set; }
        public override string SumOfSquares { get; set; }
        public override string Mean { get; set; }
        public override string StdDev { get; set; }
    }
}
