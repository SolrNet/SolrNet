using System;

namespace SolrNet {
    public static class Factory {
        private static IServiceProvider c;

        public static void Init(IServiceProvider container) {
            c = container;
        }

        public static T Get<T>() {
            return (T) c.GetService(typeof (T));
        }
    }
}