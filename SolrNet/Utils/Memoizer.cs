#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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