using static SolrNet.Impl.ResponseParsers.StatsResponseParser<double>;

namespace SolrNet
{
    /// <summary>
    /// Performs lazy casts of `TypedStatsResultString` fields to double.
    /// </summary>
    public class TypedStatsResultDouble : TypedStatsResult<double>
    {
        private readonly TypedStatsResult<string> stringValues;
        
        private double min;
        private double max;
        private double sum;
        private double sumOfSquares;
        private double mean;
        private double stdDev;
        
        public override double Min
        {
            get
            {
                if (min == default(double))
                    min = GetDoubleValue(stringValues.Min);
                return min;
            }
            set => min = value;
        }

        public override double Max
        {
            get
            {
                if (max == default(double))
                    max = GetDoubleValue(stringValues.Max);
                return max;
            }
            set => max = value;
        }

        public override double Sum
        {
            get
            {
                if (sum == default(double))
                    sum = GetDoubleValue(stringValues.Sum);
                return sum;
            }
            set => sum = value;
        }

        public override double SumOfSquares
        {
            get
            {
                if (sumOfSquares == default(double))
                    sumOfSquares = GetDoubleValue(stringValues.SumOfSquares);
                return sumOfSquares;
            }
            set => sumOfSquares = value;
        }

        public override double Mean
        {
            get
            {
                if (mean == default(double))
                    mean = GetDoubleValue(stringValues.Mean);
                return mean;
            }
            set => mean = value;
        }

        public override double StdDev
        {
            get
            {
                if (stdDev == default(double))
                    stdDev = GetDoubleValue(stringValues.StdDev);
                return stdDev;
            }
            set => stdDev = value;
        }

        public TypedStatsResultDouble(TypedStatsResult<string> stringValues)
        {
            this.stringValues = stringValues;
        }
    }
}
