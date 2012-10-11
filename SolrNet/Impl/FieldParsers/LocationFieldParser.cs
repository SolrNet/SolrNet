using System;
using System.Xml.Linq;
using System.Globalization;
using SolrNet.Exceptions;

namespace SolrNet.Impl.FieldParsers {
    public class LocationFieldParser: ISolrFieldParser {
        public bool CanHandleSolrType(string solrType) {
            return solrType == "str";
        }

        public bool CanHandleType(Type t) {
            return t == typeof(Location);
        }

        public static Location Parse(string v) {
            if (string.IsNullOrEmpty(v))
                return null;
            var m = v.Split(',');
            if (m.Length != 2)
                throw new SolrNetException(string.Format("Invalid Location '{0}'", v));
            double latitude;
            var ok = double.TryParse(m[0], NumberStyles.Any, CultureInfo.InvariantCulture, out latitude);
            if (!ok)
                throw new SolrNetException(string.Format("Invalid Location '{0}'", v));
            double longitude;
            ok = double.TryParse(m[1], NumberStyles.Any, CultureInfo.InvariantCulture, out longitude);
            if (!ok)
                throw new SolrNetException(string.Format("Invalid Location '{0}'", v));
            return new Location(latitude, longitude);
        }

        public object Parse(XElement field, Type t) {
            return Parse(field.Value);
        }
    }
}
