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

namespace SolrNet.Impl.FacetQuerySerializers {
    /// <summary>
    /// Serializes a single concrete facet query type.
    /// </summary>
    /// <typeparam name="T">Facet query type</typeparam>
    public abstract class SingleTypeFacetQuerySerializer<T> : ISolrFacetQuerySerializer {
        /// <inheritdoc/>
        public bool CanHandleType(Type t) {
            return t == typeof (T);
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, string>> Serialize(object q) {
            return Serialize((T) q);
        }

        /// <summary>
        /// Serializes a facet query
        /// </summary>
        /// <param name="q">Facet query to serialize</param>
        /// <returns>KeyValue pair representing serialized facet query</returns>
        public abstract IEnumerable<KeyValuePair<string, string>> Serialize(T q);
    }
}
