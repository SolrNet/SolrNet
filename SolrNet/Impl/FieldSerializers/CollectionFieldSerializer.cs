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
using System.Collections;
using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet.Impl.FieldSerializers {
    /// <summary>
    ///   Serializes 1-dimensional collections
    /// </summary>
    public class CollectionFieldSerializer : ISolrFieldSerializer {
        private readonly ISolrFieldSerializer valueSerializer;

        /// <summary>
        ///   Serializes 1-dimensional collections
        /// </summary>
        public CollectionFieldSerializer(ISolrFieldSerializer valueSerializer) {
            this.valueSerializer = valueSerializer;
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return t != typeof (string) &&
                   typeof (IEnumerable).IsAssignableFrom(t) &&
                   !typeof (IDictionary).IsAssignableFrom(t) &&
                   !TypeHelper.IsGenericAssignableFrom(typeof (IDictionary<,>), t);
        }

        /// <inheritdoc />
        public IEnumerable<PropertyNode> Serialize(object obj) {
            if (obj == null)
                yield break;
            foreach (var o in (IEnumerable) obj) {
                var e = valueSerializer.Serialize(o);
                if (e == null)
                    yield return new PropertyNode();
                else
                    foreach (var n in e)
                        yield return n;
            }
        }
    }
}
