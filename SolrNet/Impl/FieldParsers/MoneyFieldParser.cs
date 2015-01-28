using System;
using System.Globalization;
using System.Xml.Linq;

namespace SolrNet.Impl.FieldParsers {
    public class MoneyFieldParser : ISolrFieldParser {
        public bool CanHandleSolrType(string solrType) {
            return solrType == "str";
        }

        public bool CanHandleType(Type t) {
            return t == typeof(Money);
        }

        public static Money Parse(string v) {
            if (string.IsNullOrEmpty(v))
                return null;
            var m = v.Split(',');
            var currency = m.Length == 1 ? null : m[1];
            return new Money(decimal.Parse(m[0], CultureInfo.InvariantCulture), currency);
        }

        public object Parse(XElement field, Type t) {
            return Parse(field.Value);
        }
    }
}
