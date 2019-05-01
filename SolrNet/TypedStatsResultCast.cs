using System;
using System.Collections.Generic;

namespace SolrNet
{
    
    /// <summary>
    /// Performs lazy casts of `TypedStatsResultString` fields to type T.
    /// </summary>
    /// <typeparam name="T">The type to cast the `StatsResult` fields to.</typeparam>
    public class TypedStatsResultCast<T> : TypedStatsResult<T>
    {
        private readonly TypedStatsResult<string> stringValues;
        
        private T min;
        private T max;
        private T sum;
        private T sumOfSquares;
        private T mean;
        private T stdDev;

        public override T Min
        {
            get
            {
                if (IsDefault(min))
                    min = (T) Convert.ChangeType(stringValues.Min, typeof(T));
                return min;
            }
            set => min = value;
        }

        public override T Max
        {
            get
            {
                if (IsDefault(max))
                    max = (T) Convert.ChangeType(stringValues.Max, typeof(T));
                return max;
            }
            set => max = value;
        }

        public override T Sum
        {
            get
            {
                if (IsDefault(sum))
                    sum = (T) Convert.ChangeType(stringValues.Sum, typeof(T));
                return sum;
            }
            set => sum = value;
        }

        public override T SumOfSquares
        {
            get
            {
                if (IsDefault(sumOfSquares))
                    sumOfSquares = (T) Convert.ChangeType(stringValues.SumOfSquares, typeof(T));
                return sumOfSquares;
            }
            set => sumOfSquares = value;
        }

        public override T Mean
        {
            get
            {
                if (IsDefault(mean))
                    mean = (T) Convert.ChangeType(stringValues.Mean, typeof(T));
                return mean;
            }
            set => mean = value;
        }

        public override T StdDev
        {
            get
            {
                if (IsDefault(stdDev))
                    stdDev = (T) Convert.ChangeType(stringValues.StdDev, typeof(T));
                return stdDev;
            }
            set => stdDev = value;
        }

        public TypedStatsResultCast(TypedStatsResult<string> stringValues)
        {
            this.stringValues = stringValues;
        }

        private static readonly IEqualityComparer<T> _comparer = EqualityComparer<T>.Default;

        private static bool IsDefault(T value) => _comparer.Equals(value, default(T));
    }
}
