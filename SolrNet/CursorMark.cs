using System;

namespace SolrNet
{
    /// <summary>
    /// Use deep pagination via CursorMark.
    /// <see cref="SolrNet.Commands.Parameters.CommonQueryOptions.Start" /> must have a value of 0.
    /// <see cref="SolrNet.Commands.Parameters.QueryOptions.OrderBy" /> must contain at least one unique field
    /// and optionally any other fields.
    /// Requires Solr 4.7+
    /// </summary>
    [Serializable]
    public class CursorMark : IEquatable<CursorMark>
    {
        private readonly string mark;

        internal CursorMark(string cursorMark) {
            if (cursorMark == null)
                throw new ArgumentNullException("cursorMark");
            if (cursorMark == "")
                throw new ArgumentException("Invalid empty cursorMark", "cursorMark");
            mark = cursorMark;
        }

        /// <summary>
        /// Starting point cursor.
        /// </summary>
        public static readonly CursorMark Start = new CursorMark("*");

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CursorMark other) {
            if (other == null) return false;
            return other.mark == mark;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var m = obj as CursorMark;
            if (m == null) return false;
            return Equals(m);
        }

        /// <summary>
        /// Hash function
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return mark.GetHashCode();
        }

        /// <summary>
        /// Returns the mark of this cursor
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mark;
        }
    }
}