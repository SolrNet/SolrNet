using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet.Attributes;

namespace LINQ.SolrNet.Helpers
{
    public class FieldNameTraslator
    {
        public static string TranslateToFieldName(Type objectType, string properyName)
        {
            var pi = objectType.GetProperty(properyName);
            if (pi == null)
            {
                throw new ArgumentException(string.Format("No Property {0} in type {1}", properyName, objectType.Name));
            }

            var attrbs = pi.GetCustomAttributes(typeof(SolrFieldAttribute), false);

            var solrField = attrbs[0] as SolrFieldAttribute;

            return solrField.FieldName;
        }
    }
}
