namespace SolrNet
{
    /// <summary>
    /// ITypedStatsResult of a field.
    /// </summary>
    public interface ITypedStatsResult<out T>
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        T Min { get; }

        /// <summary>
        /// Maximum value
        /// </summary>
        T Max { get; }

        /// <summary>
        /// Sum of all values
        /// </summary>
        T Sum { get; }

        /// <summary>
        /// Sum of all values squared (useful for stddev)
        /// </summary>
        T SumOfSquares { get; }

        /// <summary>
        /// The average (v1+v2...+vN)/N
        /// </summary>
        T Mean { get; }

        /// <summary>
        /// Standard deviation
        /// </summary>
        T StdDev { get; }
    }
}
