using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SampleSolrApp.Helpers {
    public static class HtmlHelperRepeatExtensions {
        public static string RepeatF<T>(this HtmlHelper html, IEnumerable<T> items, Func<T, string> render, Func<string> separator) {
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
            var frender = ActionToFunc<T, int>(render);
            var fseparator = ActionToFunc<int>(separator);
            items.Select<T, Func<int>>(e => () => frender(e)).Intersperse(fseparator).Select(f => f()).ToArray();
        }

        // From http://blogs.msdn.com/wesdyer/archive/2007/03/09/extending-the-world.aspx
        private static IEnumerable<T> Intersperse<T>(this IEnumerable<T> sequence, T value) {
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