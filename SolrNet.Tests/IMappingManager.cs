using System;
using System.Collections.Generic;
using System.Reflection;

namespace SolrNet.Tests {
	public interface IMappingManager {
		void Add(PropertyInfo property);
		void Add(PropertyInfo property, string fieldName);

		/// <summary>
		/// Gets fields mapped for this type
		/// </summary>
		/// <param name="type"></param>
		/// <returns>Null if <paramref name="type"/> is not mapped</returns>
		ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type);

		void SetUniqueKey(PropertyInfo property);
		KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type);
	}
}