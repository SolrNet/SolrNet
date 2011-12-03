using System.Collections.Generic;

namespace SolrNet.Utils {
    public static class KV {
        public static KeyValuePair<K, V> Create<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}