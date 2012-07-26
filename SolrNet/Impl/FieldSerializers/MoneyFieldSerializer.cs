using System;
using System.Collections.Generic;

namespace SolrNet.Impl.FieldSerializers {
    public class MoneyFieldSerializer: AbstractFieldSerializer<Money> {
        public override IEnumerable<PropertyNode> Parse(Money obj) {
            yield return new PropertyNode { FieldValue = obj.ToString() };
        }
    }
}
