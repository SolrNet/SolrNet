using System;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// TermsComponent sort options
    /// </summary>
    public abstract class TermsSort : IEquatable<TermsSort> {
        internal TermsSort() {}

        /// <summary>
        /// Sorts the terms by the term frequency (highest count first)
        /// </summary>
        public static readonly TermsSort Count = new TermsSort_Count();

        /// <summary>
        /// Sorts the terms in index order.
        /// </summary>
        public static readonly TermsSort Index = new TermsSort_Index();

        private class TermsSort_Count : TermsSort {
            public override string ToString() {
                return "count";
            }
        }

        private class TermsSort_Index : TermsSort {
            public override string ToString() {
                return "index";
            }
        }

        /// <inheritdoc />
        public bool Equals(TermsSort other) {
            if (other == null)
                return false;
            return ToString() == other.ToString();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return Equals(obj as TermsSort);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}
