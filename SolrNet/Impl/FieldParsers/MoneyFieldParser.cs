using System;
using System.Globalization;
using System.Xml.Linq;

namespace SolrNet.Impl.FieldParsers {
    /// <summary>
    /// Parses a field of type <see cref="Money"/>
    /// </summary>
    public class MoneyFieldParser : ISolrFieldParser {
        /// <inheritdoc />
        public bool CanHandleSolrType(string solrType) {
            return solrType == "str";
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return t == typeof(Money);
        }

        /// <summary>
        /// Parses a field of type <see cref="Money"/>
        /// </summary>
        public static Money Parse(string v) {
            if (string.IsNullOrEmpty(v))
                return null;
            var m = v.Split(',');
            var currency = m.Length == 1 ? null : m[1];
            return new Money(decimal.Parse(m[0], CultureInfo.InvariantCulture), currency);
        }

        /// <inheritdoc />
        public object Parse(XElement field, Type t) {
            return Parse(field.Value);
        }
    }
}
