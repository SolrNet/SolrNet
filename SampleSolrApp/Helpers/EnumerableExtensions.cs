using System.Collections.Generic;
using System.Linq;

namespace SampleSolrApp.Helpers {
    public static class EnumerableExtensions {
        public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> list) {
            return list.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}