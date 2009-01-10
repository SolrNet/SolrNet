using System;
using System.Collections.Generic;
using System.Reflection;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
	/// <summary>
	/// Maps all properties in the class, with the same field name as the property.
	/// Note that unique keys must be added manually.
	/// </summary>
	public class AllPropertiesMappingManager : IReadOnlyMappingManager {
		private readonly IDictionary<Type, PropertyInfo> uniqueKeys = new Dictionary<Type, PropertyInfo>();

		public ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type) {
			var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var kvProps = Func.Select(props, prop => new KeyValuePair<PropertyInfo, string>(prop, prop.Name));
			return new List<KeyValuePair<PropertyInfo, string>>(kvProps);
		}

		public KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type) {
		    try {
		        var key = uniqueKeys[type];
		        return new KeyValuePair<PropertyInfo, string>(key, key.Name);
		    } catch (KeyNotFoundException) {
		        throw new NoUniqueKeyException(type);
		    }
		}

		public void SetUniqueKey(PropertyInfo property) {
			if (property == null)
				throw new ArgumentNullException("property");
			var t = property.ReflectedType;
			uniqueKeys[t] = property;			
		}
	}
}