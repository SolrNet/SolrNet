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

        public IEnumerator<TermsResult> GetEnumerator() {
            return Terms.GetEnumerator();
        }

        public void Add(TermsResult item) {
            Terms.Add(item);
        }

        public void Clear() {
            Terms.Clear();
        }

        public bool Contains(TermsResult item) {
            return Terms.Contains(item);
        }

        public void CopyTo(TermsResult[] array, int arrayIndex) {
            Terms.CopyTo(array, arrayIndex);
        }

        public bool Remove(TermsResult item) {
            return Terms.Remove(item);
        }

        public int Count {
            get { return Terms.Count; }
        }

        public bool IsReadOnly {
            get { return Terms.IsReadOnly; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}