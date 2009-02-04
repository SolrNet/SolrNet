using System.Collections.Generic;

namespace SampleSolrApp.Helpers {
    public static class KeyValuePairExtensions {
        public static KeyValuePair<K, V> WithKey<K, V>(this KeyValuePair<K, V> kv, K newKey) {
            return new KeyValuePair<K, V>(newKey, kv.Value);
        }

        public static KeyValuePair<K, V> WithValue<K, V>(this KeyValuePair<K, V> kv, V newValue) {
            return new KeyValuePair<K, V>(kv.Key, newValue);
        }
    }
}