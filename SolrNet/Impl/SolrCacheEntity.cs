using System;

namespace SolrNet.Impl {
    [Serializable]
    public class SolrCacheEntity {
        public string Url { get; private set; }
        public string ETag { get; private set; }
        public string Data { get; private set; }

        public SolrCacheEntity(string url, string eTag, string data) {
            Url = url;
            ETag = eTag;
            Data = data;
        }
    }
}