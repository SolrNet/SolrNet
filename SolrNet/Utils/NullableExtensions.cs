using System;

namespace SolrNet.Utils {
    public static class NullableExtensions {
        public static bool IsTrue(this bool? v) {
            return v.HasValue && v.Value;
        }
    }
}
