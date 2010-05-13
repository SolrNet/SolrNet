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

using System.Collections.Generic;
using SolrNet.Exceptions;

namespace SolrNet.Impl {
    /// <summary>
    /// Indexes documents by primary key
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrDocumentIndexer<T> : ISolrDocumentIndexer<T> {
        private readonly IReadOnlyMappingManager mappingManager;

        public SolrDocumentIndexer(IReadOnlyMappingManager mappingManager) {
            this.mappingManager = mappingManager;
        }

        /// <summary>
        /// Creates an index of documents by unique key
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public IDictionary<string, T> IndexResultsByKey(IEnumerable<T> results) {
            var r = new Dictionary<string, T>();
            var uniqueKey = mappingManager.GetUniqueKey(typeof (T));
            if (uniqueKey == null)
                throw new SolrNetException(string.Format("Can't index document type '{0}', no unique key defined", typeof(T)));
            var prop = uniqueKey.Property;
            foreach (var d in results) {
                var key = prop.GetValue(d, null).ToString();
                r[key] = d;
            }
            return r;
        }
    }
}