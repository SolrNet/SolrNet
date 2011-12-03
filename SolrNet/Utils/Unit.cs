using System;

namespace SolrNet.Utils {
    /// <summary>
    /// Void as a real, usable type
    /// </summary>
    public abstract class Unit {}

    /// <summary>
    /// Function helpers
    /// </summary>
    public abstract class F {
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        public static void DoNothing<T>(T a) {}

        /// <summary>
        /// Converts an <see cref="Action"/> into a <see cref="Func{T,Unit}"/>, which is generally more composable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Func<T,Unit> ActionToFunc<T>(Action<T> action) {
            return x => {
                action(x);
                return null;
            };
        }
    }
}