using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using SolrNet.Utils;

namespace SolrNet {
	public class JsonQueryResultParser<T> : ISolrQueryResultParser<T> where T : ISolrDocument, new() {
		public ISolrQueryResults<T> Parse(string r) {
			var o = JavaScriptConvert.DeserializeObject(r) as JavaScriptObject;
			var results = new SolrQueryResults<T>();
			var responseNode = FindNode("response", o);
			var docsNode = Func.Cast<JavaScriptObject>(FindNode<IEnumerable>("docs", responseNode));
			results.NumFound = (int)FindNode<long>("numFound", responseNode);
			foreach (var doc in docsNode) {
				var ndoc = new T();
				foreach (var field in doc) {
					var prop = GetPropertyBySolrField(field.Key);
					if (prop != null) {
						try {
							var obj = ConvertFrom(field.Value, prop.PropertyType);
							prop.SetValue(obj, ndoc, null);
						} catch (TargetException e) {
							throw new TargetException(string.Format("Couldn't set property {0}", prop.Name), e);
						}
					}
				}
				results.Add(ndoc);
			}
			return results;
		}

		public object ConvertFrom(object value, Type t) {
			if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
				var nc = new NullableConverter(t);
				return nc.ConvertFrom(value);
			}
			return Convert.ChangeType(value, t);
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
			var a = p.GetCustomAttributes(typeof(SolrFieldAttribute), true);
			if (a.Length == 0)
				return null;
			return (a[0] as SolrFieldAttribute).FieldName ?? p.Name;
		}

		public R FindNode<R>(string key, JavaScriptObject parent) {
			try {
				return (R)Func.First(Func.Filter(parent, kv => kv.Key == key)).Value;
			} catch (InvalidOperationException) {
				return default(R);
			}
		}

		public JavaScriptObject FindNode(string key, JavaScriptObject parent) {
			try {
				return Func.First(Func.Filter(parent, kv => kv.Key == key)).Value as JavaScriptObject;
			} catch (InvalidOperationException) {
				return null;
			}
		}
	}
}
