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

namespace SolrNet.Impl {
    /// <summary>
    /// Cache implementation that doesn't cache anything.
    /// Use it when you want to disable http caching.
    /// </summary>
    public class NullCache : ISolrCache {
        /// <inheritdoc />
        public SolrCacheEntity this[string url] {
            get { return null; }
        }

        /// <inheritdoc />
        public void Add(SolrCacheEntity e) {}
    }
}
