#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Impl {
    /// <summary>
    /// Spell-checking query results
    /// </summary>
    public class SpellCheckResults : ICollection<SpellCheckResult> {

        /// <summary>
        /// Multiple collations returned
        /// </summary>
        public ICollection<CollationResult> Collations = new List<CollationResult>();

        private readonly ICollection<SpellCheckResult> SpellChecks = new List<SpellCheckResult>();

        /// <inheritdoc />
        public IEnumerator<SpellCheckResult> GetEnumerator() {
            return SpellChecks.GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(SpellCheckResult item) {
            SpellChecks.Add(item);
        }

        /// <inheritdoc />
        public void Clear() {
            SpellChecks.Clear();
        }

        /// <inheritdoc />
        public bool Contains(SpellCheckResult item) {
            return SpellChecks.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(SpellCheckResult[] array, int arrayIndex) {
            SpellChecks.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(SpellCheckResult item) {
            return SpellChecks.Remove(item);
        }

        /// <inheritdoc />
        public int Count {
            get { return SpellChecks.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly {
            get { return SpellChecks.IsReadOnly; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
