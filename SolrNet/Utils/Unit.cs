using System;

namespace SolrNet.Utils {
    /// <summary>
    /// Void as a real, usable type
    /// </summary>
    public abstract class Unit {}

    /// <summary>
    /// Function helpers
    /// </summary>
    public static class F {
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
        public static Func<T, Unit> ToFunc<T>(this Action<T> action) {
            return x => {
                action(x);
                return null;
            };
        }

        public static Func<A, B, Unit> ToFunc<A, B>(this Action<A, B> action) {
            return (a, b) => {
                action(a, b);
                return null;
            };
        }

        public static Action<A, B> ToAction<A, B>(this Func<A, B, Unit> f) {
            return (a, b) => f(a, b);
        }
    }
}