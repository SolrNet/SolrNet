using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SolrNet
{
    /// <summary>
    /// Performs lazy casts of typed StatsResult fields to type T.
    /// </summary>
    /// <typeparam name="T">The type to cast the `StatsResult` fields to.</typeparam>
    public class TypedStatsResult<T> : ITypedStatsResult<T>
    {
        private Lazy<T> min;
        private Lazy<T> max;
        private Lazy<T> sum;
        private Lazy<T> sumOfSquares;
        private Lazy<T> mean;
        private Lazy<T> stdDev;

        public T Min => min.Value;
        
        public T Max => max.Value;
        
        public T Sum => sum.Value;
        
        public T SumOfSquares => sumOfSquares.Value;
        
        public T Mean => mean.Value;
        
        public T StdDev => stdDev.Value;
        
        public TypedStatsResult(ITypedStatsResult<string> stringValues)
        {
            min = new Lazy<T>(() => GetValue(stringValues.Min));
            max = new Lazy<T>(() => GetValue(stringValues.Max));
            sum = new Lazy<T>(() => GetValue(stringValues.Sum));
            sumOfSquares = new Lazy<T>(() => GetValue(stringValues.SumOfSquares));
            mean = new Lazy<T>(() => GetValue(stringValues.Mean));
            stdDev = new Lazy<T>(() => GetValue(stringValues.StdDev));
        }
        
        private static readonly TypeConverter Converter = TypeDescriptor.GetConverter(typeof(T));

        private static T GetValue(string stringValue) => stringValue != null ? (T) Converter.ConvertFromInvariantString(stringValue) : default(T);
    }
}
