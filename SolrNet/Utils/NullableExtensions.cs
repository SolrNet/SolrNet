using System;

namespace SolrNet.Utils {
    /// <summary>
    /// Extensions around nullable types
    /// </summary>
    public static class NullableExtensions {
        /// <summary>
        /// Returns true if <paramref name="v"/> has a value and is true
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsTrue(this bool? v) {
            return v.HasValue && v.Value;
        }
    }
}
