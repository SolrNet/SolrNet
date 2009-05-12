#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using SolrNet.Utils;

namespace SolrNet.Mapping {
    /// <summary>
    /// Memoizing decorator for a mapping manager
    /// </summary>
    public class MemoizingMappingManager : IReadOnlyMappingManager {
        private readonly Converter<Type, ICollection<KeyValuePair<PropertyInfo, string>>> memoGetFields;
        private readonly Converter<Type, KeyValuePair<PropertyInfo, string>> memoGetUniqueKey;

        public MemoizingMappingManager(IReadOnlyMappingManager mapper) {
            memoGetFields = Memoizer.Memoize<Type, ICollection<KeyValuePair<PropertyInfo, string>>>(mapper.GetFields);
            memoGetUniqueKey = Memoizer.Memoize<Type, KeyValuePair<PropertyInfo, string>>(mapper.GetUniqueKey);
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