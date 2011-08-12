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
    public class CollationResults : ICollection<CollationResult> {
        /// <summary>
        /// Suggestion query from spell-checking
        /// </summary>
   

        private readonly ICollection<CollationResult> Collations = new List<CollationResult>();

        public IEnumerator<CollationResult> GetEnumerator()
        {
            return Collations.GetEnumerator();
        }

        public void Add(CollationResult item)
        {
            Collations.Add(item);
        }

        public void Clear() {
            Collations.Clear();
        }

        public bool Contains(CollationResult item)
        {
            return Collations.Contains(item);
        }

        public void CopyTo(CollationResult[] array, int arrayIndex)
        {
            Collations.CopyTo(array, arrayIndex);
        }

        public bool Remove(CollationResult item)
        {
            return Collations.Remove(item);
        }

        public int Count {
            get { return Collations.Count; }
        }

        public bool IsReadOnly {
            get { return Collations.IsReadOnly; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}