using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SolrNet.Impl.FieldSerializers {
    public class TypeConvertingFieldSerializer : ISolrFieldSerializer {
        public bool CanHandleType(Type t) {
            var converter = TypeDescriptor.GetConverter(t);
            return converter.CanConvertTo(typeof (string));
        }

        public IEnumerable<PropertyNode> Serialize(object obj) {
            if (obj == null)
                yield break;
            var converter = TypeDescriptor.GetConverter(obj.GetType());
            yield return new PropertyNode {
                FieldValue = converter.ConvertToInvariantString(obj)
            };
        }
    }
}