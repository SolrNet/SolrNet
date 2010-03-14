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
using System.Web;
using System.Web.Caching;

namespace SolrNet.Impl {
    /// <summary>
    /// Uses the HttpRuntime (ASP.NET) cache
    /// </summary>
    public class HttpRuntimeCache : ISolrCache {
        /// <summary>
        /// Cache sliding minutes. By default 10 minutes
        /// </summary>
        public int SlidingMinutes { get; set; }

        private readonly string id = Guid.NewGuid().ToString();

        public HttpRuntimeCache() {
            SlidingMinutes = 10;
        }

        public string CacheKey(string url) {
            return "solr" + id + url;
        }

        public SolrCacheEntity this[string url] {
            get { return HttpRuntime.Cache[CacheKey(url)] as SolrCacheEntity; }
        }

        public void Add(SolrCacheEntity e) {
            HttpRuntime.Cache.Insert(CacheKey(e.Url), e, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, SlidingMinutes, 0));
        }
    }
}