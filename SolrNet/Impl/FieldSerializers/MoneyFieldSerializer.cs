using System;
using System.Collections.Generic;

namespace SolrNet.Impl.FieldSerializers {
    /// <summary>
    /// Serializes fields of type <see cref="Money"/>
    /// </summary>
    public class MoneyFieldSerializer: AbstractFieldSerializer<Money> {
        /// <inheritdoc />
        public override IEnumerable<PropertyNode> Parse(Money obj) {
            yield return new PropertyNode { FieldValue = obj.ToString() };
        }
    }
}
