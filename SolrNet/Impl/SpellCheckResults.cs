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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;

namespace SolrNet.Impl {
    /// <summary>
    /// Spell-checking query results
    /// </summary>
    public class SpellCheckResults : ICollection<SpellCheckResult> {
        /// <summary>
        /// First founs suggestion query from spell-checking
        /// Works if request was for extended and non-extended collation results
        /// </summary>
        public string Collation { get; set; }

        /// <summary>
        /// Extended collated suggestions from spell-checking
        /// </summary>
        public ICollection<ExtendedSpellCheckCollationResult> ExtendedCollations = new List<ExtendedSpellCheckCollationResult>();

        /// <summary>
        /// Collated query suggestions from spell-checking
        /// </summary>
        public ICollection<string> Collations = new List<string>();

        /// <summary>
        /// Whether the original query was correctly spelled
        /// </summary>
        public bool? CorrectlySpelled { get; set; }

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