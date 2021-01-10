#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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
using SolrNet.Utils;

namespace SolrNet.Mapping {
    /// <summary>
    /// Memoizing decorator for a mapping manager
    /// </summary>
    public class MemoizingMappingManager : IReadOnlyMappingManager {
        private readonly Converter<Type, IDictionary<string,SolrFieldModel>> memoGetFields;
        private readonly Converter<Type, SolrFieldModel> memoGetUniqueKey;
        private readonly IReadOnlyMappingManager mapper;
        private ICollection<Type> registeredTypes;
        private readonly object registeredTypesLock = new object();

        public MemoizingMappingManager(IReadOnlyMappingManager mapper) {
            memoGetFields = Memoizer.Memoize<Type, IDictionary<string,SolrFieldModel>>(mapper.GetFields);
            memoGetUniqueKey = Memoizer.Memoize<Type, SolrFieldModel>(mapper.GetUniqueKey);
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets fields mapped for this type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Null if <paramref name="type"/> is not mapped</returns>
        public IDictionary<string,SolrFieldModel> GetFields(Type type) {
            return memoGetFields(type);
        }

        /// <inheritdoc />
        public SolrFieldModel GetUniqueKey(Type type) {
            return memoGetUniqueKey(type);
        }

        /// <inheritdoc />
        public ICollection<Type> GetRegisteredTypes() {
            lock (registeredTypesLock) {
                if (registeredTypes == null)
                    registeredTypes = mapper.GetRegisteredTypes();
                return new List<Type>(registeredTypes);
            }
        }
    }
}
