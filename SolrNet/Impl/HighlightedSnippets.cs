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
    /// Highlighted snippets by field
    /// </summary>
    public class HighlightedSnippets : IDictionary<string, ICollection<string>> {
        /// <summary>
        /// Highlighted snippets by field
        /// </summary>
        public IDictionary<string, ICollection<string>> Snippets {
            get { return fields; }
        }

        private readonly IDictionary<string, ICollection<string>> fields = new Dictionary<string, ICollection<string>>();

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, ICollection<string>>> GetEnumerator() {
            return fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<string, ICollection<string>> item) {
            fields.Add(item);
        }

        /// <inheritdoc />
        public void Clear() {
            fields.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<string, ICollection<string>> item) {
            return fields.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<string, ICollection<string>>[] array, int arrayIndex) {
            fields.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<string, ICollection<string>> item) {
            return fields.Remove(item);
        }

        /// <inheritdoc />
        public int Count {
            get { return fields.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly {
            get { return fields.IsReadOnly; }
        }

        /// <inheritdoc />
        public bool ContainsKey(string key) {
            return fields.ContainsKey(key);
        }

        /// <inheritdoc />
        public void Add(string key, ICollection<string> value) {
            fields.Add(key, value);
        }

        /// <inheritdoc />
        public bool Remove(string key) {
            return fields.Remove(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(string key, out ICollection<string> value) {
            return fields.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public ICollection<string> this[string key] {
            get { return fields[key]; }
            set { fields[key] = value; }
        }

        /// <inheritdoc />
        public ICollection<string> Keys {
            get { return fields.Keys; }
        }

        /// <inheritdoc />
        public ICollection<ICollection<string>> Values {
            get { return fields.Values; }
        }
    }
}
