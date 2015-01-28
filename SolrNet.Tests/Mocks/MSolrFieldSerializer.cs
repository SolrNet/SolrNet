using System;
using System.Collections.Generic;
using SolrNet.Impl;

namespace SolrNet.Tests.Mocks {
    class MSolrFieldSerializer: ISolrFieldSerializer {
        public Func<Type, bool> canHandleType;
        public Func<object, IEnumerable<PropertyNode>> serialize;

        public bool CanHandleType(Type t) {
            return canHandleType(t);
        }

        public IEnumerable<PropertyNode> Serialize(object obj) {
            return serialize(obj);
        }
    }
}
