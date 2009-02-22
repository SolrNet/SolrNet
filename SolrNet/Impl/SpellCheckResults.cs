using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Impl {
    public class SpellCheckResults : ICollection<SpellCheckResult> {
        public string Collation { get; set; }
        private readonly ICollection<SpellCheckResult> SpellChecks = new List<SpellCheckResult>();

        public IEnumerator<SpellCheckResult> GetEnumerator() {
            return SpellChecks.GetEnumerator();
        }

        public void Add(SpellCheckResult item) {
            SpellChecks.Add(item);
        }

        public void Clear() {
            SpellChecks.Clear();
        }

        public bool Contains(SpellCheckResult item) {
            return SpellChecks.Contains(item);
        }

        public void CopyTo(SpellCheckResult[] array, int arrayIndex) {
            SpellChecks.CopyTo(array, arrayIndex);
        }

        public bool Remove(SpellCheckResult item) {
            return SpellChecks.Remove(item);
        }

        public int Count {
            get { return SpellChecks.Count; }
        }

        public bool IsReadOnly {
            get { return SpellChecks.IsReadOnly; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}