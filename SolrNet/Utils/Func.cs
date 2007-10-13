using System;
using System.Collections.Generic;

namespace SolrNet.Utils {
	/// <summary>
	/// From http://diditwith.net/PermaLink,guid,a1a76478-03d2-428f-9db6-9cf4e300ea0f.aspx
	/// </summary>
	public class Func {
		public delegate TResult Accumulator<TSource, TResult>(TSource x, TResult y);

		public static TResult Reduce<TSource, TResult>(IEnumerable<TSource> source, TResult startValue,
		                                               Accumulator<TSource, TResult> accumulator) {
			TResult result = startValue;
			if (source != null) {
				foreach (TSource item in source)
					result = accumulator(item, result);
			}

			return result;
		}

		public static IEnumerable<TSource> Filter<TSource>(IEnumerable<TSource> source, Predicate<TSource> predicate) {
			return Reduce(source, new List<TSource>(),
			              delegate(TSource item, List<TSource> result) {
			              	if (predicate(item))
			              		result.Add(item);

			              	return result;
			              });
		}

		public static IEnumerable<TResult> Map<TSource, TResult>(IEnumerable<TSource> source,
		                                                         Converter<TSource, TResult> converter) {
			return Reduce(source, new List<TResult>(),
			              delegate(TSource item, List<TResult> result) {
			              	result.Add(converter(item));

			              	return result;
			              });
		}
	}
}