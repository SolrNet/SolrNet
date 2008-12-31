using System;
using System.Collections.Generic;
using System.Reflection;
using SolrNet.Utils;

namespace SolrNet {
    public class MemoizingMappingManager : IReadOnlyMappingManager {
        private readonly Converter<Type, ICollection<KeyValuePair<PropertyInfo, string>>> memoGetFields;
        private readonly Converter<Type, KeyValuePair<PropertyInfo, string>> memoGetUniqueKey;

        public MemoizingMappingManager(IReadOnlyMappingManager mapper) {
            memoGetFields = Memoizer.Memoize<Type, ICollection<KeyValuePair<PropertyInfo, string>>>(t => mapper.GetFields(t));
            memoGetUniqueKey = Memoizer.Memoize<Type, KeyValuePair<PropertyInfo, string>>(t => mapper.GetUniqueKey(t));
        }

        /// <summary>
        /// Gets fields mapped for this type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Null if <paramref name="type"/> is not mapped</returns>
        public ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type) {
            return memoGetFields(type);
        }

        public KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type) {
            return memoGetUniqueKey(type);
        }
    }
}