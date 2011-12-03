using System.Collections.Generic;

namespace SolrNet.Utils {
    /// <summary>
    /// Helper KeyValuePair constructor
    /// </summary>
    public static class KV {
        /// <summary>
        /// Helper KeyValuePair constructor
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static KeyValuePair<K, V> Create<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}