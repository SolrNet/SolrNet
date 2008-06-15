using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SolrNet.Utils {
	/// <summary>
	/// From http://diditwith.net/PermaLink,guid,a1a76478-03d2-428f-9db6-9cf4e300ea0f.aspx
	/// </summary>
	public class Func {
		public delegate TResult Accumulator<TSource, TResult>(TSource x, TResult y);

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
			return Reduce(source, new List<TSource>(),
			              (item, result) => {
			              	if (predicate(item))
			              		result.Add(item);

			              	return result;
			              });
		}

		public static IEnumerable<TResult> Map<TSource, TResult>(IEnumerable<TSource> source,
		                                                         Converter<TSource, TResult> converter) {
			return Reduce(source, new List<TResult>(),
			              (item, result) => {
			              	result.Add(converter(item));

			              	return result;
			              });
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
				yield return e.Current;
			}
		}

		public static IEnumerable<R> Select<R>(IEnumerable l, Converter<object, R> f) {
			foreach (var e in l) {
				yield return f(e);
			}
		}

		public static IEnumerable<R> Select<T, R>(IEnumerable<T> l, Converter<T, R> f) {
			foreach (var e in l) {
				yield return f(e);
			}
		}
	}
}