using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace SolrNet {
	public class JsonQueryResultParser<T> : ISolrQueryResultParser<T> where T : ISolrDocument, new() {
		public ISolrQueryResults<T> Parse(string r) {
			var o = JavaScriptConvert.DeserializeObject(r) as JavaScriptObject;
			var results = new SolrQueryResults<T>();
			var responseNode = FindNode("response", o);
			var docsNode = Q.Cast<JavaScriptObject>(FindNode<IEnumerable>("docs", responseNode));
			results.NumFound = (int)FindNode<long>("numFound", responseNode);
			foreach (var doc in docsNode) {
				var ndoc = new T();
				foreach (var field in doc) {
					var prop = GetPropertyBySolrField(field.Key);
					if (prop != null)
						prop.SetValue(Convert.ChangeType(field.Value, prop.PropertyType), ndoc, null);
				}
				results.Add(ndoc);
			}
			return results;
		}

		public PropertyInfo GetPropertyBySolrField(string field) {
			foreach (var prop in typeof(T).GetProperties()) {
				var fname = GetSolrFieldName(prop);
				if (fname != null && fname == field)
					return prop;
			}
			return null;
		}

		public string GetSolrFieldName(PropertyInfo p) {
			var a = p.GetCustomAttributes(typeof (SolrFieldAttribute), true);
			if (a.Length == 0)
				return null;
			return (a[0] as SolrFieldAttribute).FieldName ?? p.Name;
		}

		public R FindNode<R>(string key, JavaScriptObject parent) {
			try {
				return (R)Q.First(Q.Where(kv => kv.Key == key, parent)).Value;
			} catch {
				return default(R);
			}			
		}

		public JavaScriptObject FindNode(string key, JavaScriptObject parent) {
			try {
				return Q.First(Q.Where(kv => kv.Key == key, parent)).Value as JavaScriptObject;
			} catch {
				return null;
			}
		}
	}

	public static class Q {
		public delegate R Func<P, R>(P e);

		public static IEnumerable<R> Cast<R>(IEnumerable e) {
			foreach (var i in e)
				yield return (R) Convert.ChangeType(i, typeof(R));
		}

		public static T First<T>(IEnumerable<T> e) {
			foreach (var i in e)
				return i;
			throw new ApplicationException();
		}

		public static IEnumerable<R> Select<T, R>(Func<T, R> f, IEnumerable<T> e) {
			foreach (var i in e)
				yield return f(i);
		}

		public static IEnumerable<T> Where<T>(Func<T, bool> condition, IEnumerable<T> e) {
			foreach (var i in e)
				if (condition(i))
					yield return i;
		}
	}
}