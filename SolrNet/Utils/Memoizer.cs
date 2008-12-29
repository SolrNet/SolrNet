using System;
using System.Collections.Generic;

namespace SolrNet.Utils {
    public class Memoizer {
        public static Converter<TArg, TResult> Memoize<TArg, TResult>(Converter<TArg, TResult> function) {
            var results = new Dictionary<TArg, TResult>();

            return key => {
                lock (results) {
                    TResult value;
                    if (results.TryGetValue(key, out value))
                        return value;

                    value = function(key);
                    results.Add(key, value);
                    return value;
                }
            };
        }
    }
}