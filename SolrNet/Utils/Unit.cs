using System;

namespace SolrNet.Utils {
    public abstract class Unit {}

    public abstract class F {
        public static void DoNothing<T>(T a) {}

        public static Func<T,Unit> ActionToFunc<T>(Action<T> action) {
            return x => {
                action(x);
                return null;
            };
        }
    }
}