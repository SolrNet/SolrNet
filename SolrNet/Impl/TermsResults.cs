using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Impl {
    /// <summary>
    /// Terms results
    /// </summary>
    public class TermsResults : ICollection<TermsResult> {
        /// <summary>
        /// Terms results
        /// </summary>

        private readonly ICollection<TermsResult> Terms = new List<TermsResult>();

        /// <inheritdoc />
        public IEnumerator<TermsResult> GetEnumerator() {
            return Terms.GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(TermsResult item) {
            Terms.Add(item);
        }

        /// <inheritdoc />
        public void Clear() {
            Terms.Clear();
        }

        /// <inheritdoc />
        public bool Contains(TermsResult item) {
            return Terms.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(TermsResult[] array, int arrayIndex) {
            Terms.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(TermsResult item) {
            return Terms.Remove(item);
        }

        /// <inheritdoc />
        public int Count {
            get { return Terms.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly {
            get { return Terms.IsReadOnly; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
