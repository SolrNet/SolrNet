namespace SolrNet.Impl {
    /// <summary>
    /// Cache implementation that doesn't cache anything.
    /// Use it when you want to disable http caching.
    /// </summary>
    public class NullCache : ISolrCache {
        public SolrCacheEntity this[string url] {
            get { return null; }
        }

        public void Add(SolrCacheEntity e) {}
    }
}