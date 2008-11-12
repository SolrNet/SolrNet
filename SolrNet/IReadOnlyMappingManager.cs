using System;
using System.Collections.Generic;
using System.Reflection;

namespace SolrNet {
	public interface IReadOnlyMappingManager {
		/// <summary>
		/// Gets fields mapped for this type
		/// </summary>
		/// <param name="type"></param>
		/// <returns>Null if <paramref name="type"/> is not mapped</returns>
		ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type);

		KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type);
	}
}