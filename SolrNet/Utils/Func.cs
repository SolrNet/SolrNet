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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SolrNet.Utils {
	/// <summary>
	/// Functional utilities
	/// Mostly from http://diditwith.net/PermaLink,guid,a1a76478-03d2-428f-9db6-9cf4e300ea0f.aspx
	/// </summary>
	public class Func {
		public delegate TResult Accumulator<TSource, TResult>(TSource x, TResult y);

	    public delegate T Function<T>();
        public delegate TResult Func2<TArg1, TArg2, TResult>(TArg1 a, TArg2 b);

		public static TResult Reduce<TSource, TResult>(IEnumerable<TSource> source, TResult startValue,
		                                               Accumulator<TSource, TResult> accumulator) {
			var result = startValue;
			if (source != null) {
				foreach (var item in source)
					result = accumulator(item, result);
			}

			return result;
		}

		public static IEnumerable<TSource> Filter<TSource>(IEnumerable<TSource> source, Predicate<TSource> predicate) {
			foreach (var s in source) {
				if (predicate(s))
					yield return s;
			}
		}

        public static bool IsEmpty(IEnumerable e) {
            foreach (var i in e)
                return false;
            return true;
        }

		public static T First<T>(IEnumerable<T> e) {
			foreach (var i in e)
				return i;
			throw new InvalidOperationException();
		}

        public static T FirstOrDefault<T>(IEnumerable<T> e) {
            foreach (var i in e)
                return i;
            return default(T);
        }

        public static T FirstOrDefault<T>(IEnumerable<T> e, Predicate<T> condition) {
            foreach (var i in e)
                if (condition(i))
                    return i;
            return default(T);
        }


		/// <summary>
		/// This is a conversion cast, unlike LINQ's Cast()
		/// </summary>
		/// <typeparam name="R"></typeparam>
		/// <param name="e"></param>
		/// <returns></returns>
		public static IEnumerable<R> Convert<R>(IEnumerable e) {
			foreach (var i in e)
				yield return (R)System.Convert.ChangeType(i, typeof(R));
		}

		public static IEnumerable<R> Cast<R>(IEnumerable e) {
			foreach (var i in e)
				yield return (R) i;
		}

		// These two are from http://weblogs.asp.net/whaggard/archive/2004/12/06/275917.aspx
		// Default join that takes an IEnumerable list and just takes the ToString of each item
		public static string Join<T>(string separator, IEnumerable<T> list) {
			return Join(separator, list, o => o.ToString());
		}

		// Join that takes an IEnumerable list that uses a converter to convert the type to a string
		public static string Join<T>(string separator, IEnumerable<T> list, Converter<T, string> converter) {
			return Join(separator, list, converter, false);
		}

        public static string Join<T>(IEnumerable<T> list) {
            return Join("", list);
        }

		// Join that takes an IEnumerable list that uses a converter to convert the type to a string
		public static string Join<T>(string separator, IEnumerable<T> list, Converter<T, string> converter, bool ignoreNulls) {
			var sb = new StringBuilder();
			foreach (T t in list) {
				if (ignoreNulls && t == null)
					continue;
				if (sb.Length != 0)
					sb.Append(separator);
				sb.Append(converter(t));
			}
			return sb.ToString();
		}

		public static IEnumerable<T> Take<T>(IEnumerable<T> l, int count) {
			var e = l.GetEnumerator();
			var c = 0;
			while (e.MoveNext() && c < count) {
				c++;
				yield return e.Current;
			}
		}

		public static IEnumerable<R> Select<T, R>(IEnumerable<T> l, Converter<T, R> f) {
			foreach (var e in l) {
				yield return f(e);
			}
		}

		public static T[] ToArray<T>(IEnumerable<T> l) {
			var r = new List<T>();
			foreach (var o in l)
				r.Add(o);
			return r.ToArray();
		}

        public static bool Any<T>(IEnumerable<T> l, Converter<T, bool> condition) {
            foreach (var p in l)
                if (condition(p))
                    return true;
            return false;
        }

        public static IEnumerable<T> Distinct<T>(IEnumerable<T> l) {
            var enumerated = new List<T>();
            foreach (var e in l) {
                if (enumerated.Contains(e))
                    continue;
                enumerated.Add(e);
                yield return e;
            }
        }

        public static IEnumerable<T> Skip<T>(IEnumerable<T> l, int skipCount) {
            foreach (var e in l)
                if (skipCount-- <= 0)
                    yield return e;
        }

        public static IEnumerable<T> Tail<T>(IEnumerable<T> l) {
            return Skip(l, 1);
        }
	}
}