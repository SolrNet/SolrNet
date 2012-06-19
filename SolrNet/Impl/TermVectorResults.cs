using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Impl {
    /// <summary>
    /// TermVector results
    /// </summary>
    public class TermVectorResults : ICollection<TermVectorDocumentResult> {
        /// <summary>
        /// TermVector results
        /// </summary>
        private readonly IList<TermVectorDocumentResult> TermVectors = new List<TermVectorDocumentResult>();


        public IEnumerator<TermVectorDocumentResult> GetEnumerator() {
            return TermVectors.GetEnumerator();
        }

        public void Add(TermVectorDocumentResult item) {
            TermVectors.Add(item);
        }

        public void Clear() {
            TermVectors.Clear();
        }

        public bool Contains(TermVectorDocumentResult item) {
            return TermVectors.Contains(item);
        }

        public void CopyTo(TermVectorDocumentResult[] array, int arrayIndex) {
            TermVectors.CopyTo(array, arrayIndex);
        }

        public bool Remove(TermVectorDocumentResult item) {
            return TermVectors.Remove(item);
        }

        public int Count {
            get { return TermVectors.Count; }
        }

        public bool IsReadOnly {
            get { return TermVectors.IsReadOnly; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}