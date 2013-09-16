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
using System.Linq;
using System.Web.Mvc;

namespace SampleSolrApp.Helpers {
    public static class HtmlHelperRepeatExtensions {
        public static string RepeatF<T>(this HtmlHelper html, IEnumerable<T> items, Func<T, string> render, Func<string> separator) {
            if (items == null)
                items = Enumerable.Empty<T>();
            return string.Join(separator(), items.Select(render).ToArray());
        }

        private static Func<T, U> ActionToFunc<T, U>(Action<T> a) {
            return t => {
                a(t);
                return default(U);
            };
        }

        private static Func<T> ActionToFunc<T>(Action a) {
            return () => {
                a();
                return default(T);
            };
        }

        public static void Repeat<T>(this HtmlHelper html, IEnumerable<T> items, Action<T> render, Action separator) {
            if (items == null)
                items = Enumerable.Empty<T>();
            var frender = ActionToFunc<T, int>(render);
            var fseparator = ActionToFunc<int>(separator);
            items.Select<T, Func<int>>(e => () => frender(e)).Intersperse(fseparator).Select(f => f()).ToArray();
        }

        // From http://blogs.msdn.com/wesdyer/archive/2007/03/09/extending-the-world.aspx
        private static IEnumerable<T> Intersperse<T>(this IEnumerable<T> sequence, T value) {
            if (sequence == null)
                throw new ArgumentNullException("sequence");
            bool first = true;
            foreach (var item in sequence) {
                if (first)
                    first = false;
                else
                    yield return value;
                yield return item;
            }
        }
    }
}