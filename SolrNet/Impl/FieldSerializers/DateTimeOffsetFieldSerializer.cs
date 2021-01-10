using System;
using System.Collections.Generic;
using System.Globalization;

namespace SolrNet.Impl.FieldSerializers {
    public class DateTimeOffsetFieldSerializer : AbstractFieldSerializer<DateTimeOffset> {
        public static string Serialize(DateTimeOffset dt) {
            return dt.ToUniversalTime().ToString(DateTimeFieldSerializer.DateTimeFormat, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public override IEnumerable<PropertyNode> Parse(DateTimeOffset obj) {
            yield return new PropertyNode {
                FieldValue = Serialize(obj),
            };
        }
    }
}
