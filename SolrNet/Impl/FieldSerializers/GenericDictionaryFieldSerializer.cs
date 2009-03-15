using System;
using System.Collections;
using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet.Impl.FieldSerializers {
    public class GenericDictionaryFieldSerializer : ISolrFieldSerializer {
        private readonly ISolrFieldSerializer serializer;

        public GenericDictionaryFieldSerializer(ISolrFieldSerializer serializer) {
            this.serializer = serializer;
        }

        public bool CanHandleType(Type t) {
            return TypeHelper.IsGenericAssignableFrom(typeof (IDictionary<,>), t);
        }

        public string KVKey(object kv) {
            return kv.GetType().GetProperty("Key").GetValue(kv, null).ToString();
        }

        public string KVValue(object kv) {
            var value = kv.GetType().GetProperty("Value").GetValue(kv, null);
            return value == null ? null : value.ToString();
        }

        public IEnumerable<PropertyNode> Serialize(object obj) {
            foreach (var de in (IEnumerable)obj) {
                var name = KVKey(de); 
                var value = serializer.Serialize(KVValue(de));
                foreach (var v in value)
                    yield return new PropertyNode {
                        FieldValue = v.FieldValue,
                        FieldNameSuffix = name,
                    };
            }
        }
    }
}