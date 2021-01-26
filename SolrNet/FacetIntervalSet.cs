using SolrNet.Impl.FieldSerializers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SolrNet
{
    public class FacetIntervalSet
    {
        private static Regex cleanCharacters = new Regex("([,[\\]()])", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        /// <summary>
        /// Creates a new unbound FacetIntervalSet
        /// </summary>
        public FacetIntervalSet() { }

        /// <summary>
        /// Creates a new FacetIntervalSet
        /// </summary>
        /// <param name="start">Start of the interval, null for unbound</param>
        /// <param name="end">End of the interval, null for unbound</param>
        public FacetIntervalSet(FacetIntervalSetValue start, FacetIntervalSetValue end) : this(start, end, new LocalParams())
        {

        }

        public FacetIntervalSet(FacetIntervalSetValue start, FacetIntervalSetValue end, LocalParams localParams)
        {
            Start = start;
            End = end;
            LocalParams = localParams;
        }

        /// <summary>
        /// Creates a new FacetIntervalSet
        /// </summary>
        /// <param name="start">Start of the interval, null for unbound</param>
        /// <param name="end">End of the interval, null for unbound</param>
        /// <param name="key">The key to use as facet key.</param>
        public FacetIntervalSet(FacetIntervalSetValue start, FacetIntervalSetValue end, string key) : this(start, end, new LocalParams(new Dictionary<string, string>() { { "key", key } }))
        {
        }

        /// <summary>
        /// Start of interval set, leave Null to have an unbounded start.
        /// </summary>
        public FacetIntervalSetValue Start { get; set; }

        /// <summary>
        /// End of interval set, leave Null to have an unbounded end.
        /// </summary>
        public FacetIntervalSetValue End { get; set; }

        /// <summary>
        /// Can be used to set a key to the set.
        /// </summary>
        public LocalParams LocalParams { get; set; } = new LocalParams();

        /// <inheritdoc />
        public override string ToString()
        {

            string result = $"{SetValueToString(Start, true)},{SetValueToString(End, false)}";

            if (LocalParams != null)
            {
                result = LocalParams.ToString() + result;
            }

            return result;

        }

        private string SetValueToString(FacetIntervalSetValue value, bool isStart)
        {
            char inclusive = isStart ? '[' : ']';
            char exclusive = isStart ? '(' : ')';
            string unbound = "*";


            if (isStart)
            {
                return ((value?.Inclusive).GetValueOrDefault(true) ? inclusive : exclusive)
                     + cleanCharacters.Replace(value?.Value ?? unbound, @"\$1");
            }
            else
            {
                return cleanCharacters.Replace(value?.Value ?? unbound, @"\$1")
                    + ((value?.Inclusive).GetValueOrDefault(true) ? inclusive : exclusive);
            }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj?.ToString());
        }
    }

    public class FacetIntervalSetValue
    {
        /// <summary>
        /// Create a new Set value
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inclusive">Whether the range includes the value.</param>
        public FacetIntervalSetValue(int value, bool inclusive = false) : this(value.ToString(), inclusive)
        {

        }

        public FacetIntervalSetValue(DateTime value, bool inclusive = false) : this(DateTimeFieldSerializer.SerializeDate(value), inclusive)
        {

        }


        public FacetIntervalSetValue(string value, bool inclusive = false)
        {
            this.Value = value;
            this.Inclusive = inclusive;

        }


        public string Value { get; }
        public bool Inclusive { get; }


    }
}
