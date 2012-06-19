using System;
using System.Xml.Linq;

namespace SolrNet.Utils {
    public static class XAttributeExtensions {
        public static string ValueOrNull(this XAttribute attr) {
            return attr == null ? null : attr.Value;
        }
    }
}
