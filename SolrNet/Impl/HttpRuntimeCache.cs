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