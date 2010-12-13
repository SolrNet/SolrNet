#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;

namespace SolrNet.Utils {
    /// <summary>
    /// Function memoizer
    /// From http://blogs.msdn.com/wesdyer/archive/2007/01/26/function-memoization.aspx
    /// </summary>
    public class Memoizer {
        /// <summary>
        /// Function memoizer
        /// From http://blogs.msdn.com/wesdyer/archive/2007/01/26/function-memoization.aspx
        /// </summary>
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

        private struct Tuple2<A,B> {
            private readonly A first;
            private readonly B second;

            public Tuple2(A first, B second) {
                this.first = first;
                this.second = second;
            }

            public bool Equals(Tuple2<A, B> other) {
                return Equals(other.first, first) && Equals(other.second, second);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != typeof (Tuple2<A, B>)) return false;
                return Equals((Tuple2<A, B>) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return (first.GetHashCode()*397) ^ second.GetHashCode();
                }
            }
        }

        public static Func<TArg1, TArg2, TResult> Memoize2<TArg1, TArg2, TResult>(Func<TArg1, TArg2, TResult> function) {
            var results = new Dictionary<Tuple2<TArg1, TArg2>, TResult>();

            return (k1, k2) => {
                lock (results) {
                    TResult value;
                    var tupleKey = new Tuple2<TArg1, TArg2>(k1, k2);
                    if (results.TryGetValue(tupleKey, out value))
                        return value;

                    value = function(k1, k2);
                    results.Add(tupleKey, value);
                    return value;
                }               
            };
        }
    }
}