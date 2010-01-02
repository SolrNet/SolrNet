namespace SolrNet.Impl {
    /// <summary>
    /// HTTP cache
    /// </summary>
    public interface ISolrCache {
        SolrCacheEntity this[string url] { get; }
        void Add(SolrCacheEntity e);
    }
}