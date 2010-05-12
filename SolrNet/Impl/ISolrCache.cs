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
    /// HTTP-level cache for Solr responses
    /// </summary>
    public interface ISolrCache {
        /// <summary>
        /// Gets a cached Solr response. Returns null if there is no cached response for this URL
        /// </summary>
        /// <param name="url">Full Solr query URL (including all querystring parameters)</param>
        /// <returns></returns>
        SolrCacheEntity this[string url] { get; }

        /// <summary>
        /// Adds a Solr response to the cache
        /// </summary>
        /// <param name="e"></param>
        void Add(SolrCacheEntity e);
    }
}