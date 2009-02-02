using System.Collections.Generic;
using System.Linq;

namespace SampleSolrApp.Helpers {
    public static class ObjectExtensions {
        public static string ToNullOrString(this object o) {
            return o == null ? null : o.ToString();
        }

        public static IDictionary<string, object> ToPropertyDictionary(this object o) {
            if (o == null)
                return null;
            return o.GetType().GetProperties()
                .Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(o, null)))
                .ToDictionary();
        }
    }
}