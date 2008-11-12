using System;
using System.Collections.Generic;
using System.Reflection;
using SolrNet.Attributes;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
	/// <summary>
	/// Gets mapping info from attributes like <see cref="SolrFieldAttribute"/> and <see cref="SolrUniqueKeyAttribute"/>
	/// </summary>
	public class AttributesMappingManager : IReadOnlyMappingManager {
		public IEnumerable<KeyValuePair<PropertyInfo, T[]>> GetPropertiesWithAttribute<T>(Type type) where T: Attribute {
			var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var kvAttrs = Func.Select(props, prop => new KeyValuePair<PropertyInfo, T[]>(prop, GetCustomAttributes<T>(prop)));
			var propsAttrs = Func.Filter(kvAttrs, kv => kv.Value.Length > 0);
			return propsAttrs;			
		}

		public ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type) {
			var propsAttrs = GetPropertiesWithAttribute<SolrFieldAttribute>(type);
			var fields = Func.Select(propsAttrs, kv => new KeyValuePair<PropertyInfo, string>(kv.Key, kv.Value[0].FieldName ?? kv.Key.Name));
			return new List<KeyValuePair<PropertyInfo, string>>(fields);
		}

		public T[] GetCustomAttributes<T>(PropertyInfo prop) where T: Attribute {
			return (T[])prop.GetCustomAttributes(typeof(T), true);
		}

		public KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type) {
			var propsAttrs = GetPropertiesWithAttribute<SolrUniqueKeyAttribute>(type);
			var fields = Func.Select(propsAttrs, kv => new KeyValuePair<PropertyInfo, string>(kv.Key, kv.Value[0].FieldName ?? kv.Key.Name));
			try {
				return Func.First(fields);
			} catch (InvalidOperationException e) {
				throw new NoUniqueKeyException(string.Format("Type '{0}' has no unique key defined", type), e);
			}
		}
	}
}