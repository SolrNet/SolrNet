using System;

namespace SolrNet
{
    /// <summary>
    ///     Use deep pagination via Solr CursorMark
    ///     IMPORTANT. <see cref="CommonQueryOptions.Start" /> must have a value of 0.
    ///     IMPROTANT. <see cref="QueryOptions.OrderBy" /> must contain at least one UNIQUE field
    ///     and optionally any other fields.
    ///     In order to use cursorMark in SOLR there must be sorting on at least one unique field.
    ///     In addition other non uniques fields can also be used for sorting.
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

        public static readonly CursorMark Start = new CursorMark("*");

        public bool Equals(CursorMark other) {
            if (other == null) return false;
            return other.mark == mark;
        }

        public override bool Equals(object obj)
        {
            var m = obj as CursorMark;
            if (m == null) return false;
            return Equals(m);
        }

        public override int GetHashCode()
        {
            return mark.GetHashCode();
        }

        public override string ToString()
        {
            return mark;
        }
    }
}