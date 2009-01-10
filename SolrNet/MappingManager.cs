using System;
using System.Collections.Generic;
using System.Reflection;

namespace SolrNet {
	public class MappingManager : IMappingManager {
		private readonly IDictionary<Type, Dictionary<PropertyInfo, string>> mappings = new Dictionary<Type, Dictionary<PropertyInfo, string>>();
		private readonly IDictionary<Type, PropertyInfo> uniqueKeys = new Dictionary<Type, PropertyInfo>();

		public void Add(PropertyInfo property) {
			Add(property, property.Name);
		}

		public void Add(PropertyInfo property, string fieldName) {
			if (property == null)
				throw new ArgumentNullException("property");
			if (fieldName == null)
				throw new ArgumentNullException("fieldName");
			var t = property.ReflectedType;
			if (!mappings.ContainsKey(t))
				mappings[t] = new Dictionary<PropertyInfo, string>(); // new List<KeyValuePair<PropertyInfo, string>>();
			mappings[t][property] = fieldName;
		}

		/// <summary>
		/// Gets fields mapped for this type
		/// </summary>
		/// <param name="type"></param>
		/// <returns>Null if <paramref name="type"/> is not mapped</returns>
		public ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");
			if (!mappings.ContainsKey(type))
				return new KeyValuePair<PropertyInfo, string>[0];
			return mappings[type];
		}

		public void SetUniqueKey(PropertyInfo property) {
			if (property == null)
				throw new ArgumentNullException("property");
			var t = property.ReflectedType;
			if (!mappings.ContainsKey(t))
				throw new ArgumentException(string.Format("Property '{0}.{1}' not mapped. Please use Add() to map it first", t, property.Name));
			uniqueKeys[t] = property;
		}

		public KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");
			var prop = uniqueKeys[type];
			return new KeyValuePair<PropertyInfo, string>(prop, mappings[type][prop]);
		}
	}
}