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

namespace SolrNet.Impl
{
    /// <summary>
    /// Spell-checking collation results
    /// </summary>
    public class CollationResults : ICollection<CollationResult>
    {
        /// <summary>
        ///  Original query term
        /// </summary>
        public string Query { get; set; }

        private readonly ICollection<CollationResult> _collations = new List<CollationResult>();

        public IEnumerator<CollationResult> GetEnumerator()
        {
            return _collations.GetEnumerator();
        }

        public void Add(CollationResult item)
        {
            _collations.Add(item);
        }

        public void Clear() {
            _collations.Clear();
        }

        public bool Contains(CollationResult item)
        {
            return _collations.Contains(item);
        }

        public void CopyTo(CollationResult[] array, int arrayIndex)
        {
            _collations.CopyTo(array, arrayIndex);
        }

        public bool Remove(CollationResult item)
        {
            return _collations.Remove(item);
        }

        public int Count {
            get { return _collations.Count; }
        }

        public bool IsReadOnly {
            get { return _collations.IsReadOnly; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
