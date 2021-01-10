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
    /// Serializes <see cref="IDictionary{TKey,TValue}"/> properties
    /// </summary>
    public class GenericDictionaryFieldSerializer : ISolrFieldSerializer {
        private readonly ISolrFieldSerializer serializer;

        public GenericDictionaryFieldSerializer(ISolrFieldSerializer serializer) {
            this.serializer = serializer;
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return TypeHelper.IsGenericAssignableFrom(typeof (IDictionary<,>), t);
        }

        /// <summary>
        /// Gets the key from a <see cref="KeyValuePair{TKey,TValue}"/>
        /// </summary>
        /// <param name="kv"></param>
        /// <returns></returns>
        public string KVKey(object kv) {
            return kv.GetType().GetProperty("Key").GetValue(kv, null).ToString();
        }

        /// <summary>
        /// Gets the value from a <see cref="KeyValuePair{TKey,TValue}"/>
        /// </summary>
        /// <param name="kv"></param>
        /// <returns></returns>
        public object KVValue(object kv) {
            return kv.GetType().GetProperty("Value").GetValue(kv, null);
        }

        /// <inheritdoc />
        public IEnumerable<PropertyNode> Serialize(object obj) {
            if (obj == null)
                yield break;
            foreach (var de in (IEnumerable)obj) {
                var name = KVKey(de); 
                var value = serializer.Serialize(KVValue(de));
                if (value == null)
                    yield return new PropertyNode {FieldNameSuffix = name};
                else
                    foreach (var v in value)
                        yield return new PropertyNode {
                            FieldValue = v.FieldValue,
                            FieldNameSuffix = name,
                        };
            }
        }
    }
}
