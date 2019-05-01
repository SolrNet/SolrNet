namespace SolrNet
{
    /// <summary>
    /// TypedStatsResult of a field.
    ///
    /// The supported types are:
    ///  - String
    ///  - Numeric (double)
    ///  - Date (DateTimeOffset)
    ///
    /// Additional types can be supported through the `TypedStatsResultCast` which performs simple type casts. 
    /// </summary>
    public abstract class TypedStatsResult<T>
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public abstract T Min { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public abstract T Max { get; set; }

        /// <summary>
        /// Sum of all values
        /// </summary>
        public abstract T Sum { get; set; }

        /// <summary>
        /// Sum of all values squared (useful for stddev)
        /// </summary>
        public abstract T SumOfSquares { get; set; }

        /// <summary>
        /// The average (v1+v2...+vN)/N
        /// </summary>
        public abstract T Mean { get; set; }

        /// <summary>
        /// Standard deviation
        /// </summary>
        public abstract T StdDev { get; set; }
    }
}
