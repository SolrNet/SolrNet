using System;
using System.Collections.Generic;
using System.Reflection;
using SolrNet.Exceptions;

namespace SolrNet {
	public interface IReadOnlyMappingManager {
		/// <summary>
		/// Gets fields mapped for this type
		/// </summary>
		/// <param name="type"></param>
		/// <returns>Empty collection if <paramref name="type"/> is not mapped</returns>
		ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type);

        /// <summary>
        /// Gets unique key for the type
        /// </summary>
        /// <exception cref="NoUniqueKeyException">Thrown when <paramref name="type"/> has no unique key defined</exception>
        /// <param name="type"></param>
        /// <returns></returns>
		KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type);
	}
}