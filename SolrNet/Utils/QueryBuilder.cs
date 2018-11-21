using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SolrNet.Utils
{
    public static class QueryBuilder
    {
        public static string GetQuery(IEnumerable<KeyValuePair<string, string>> parameters, string version = "n/a")
        {
            var param = new List<KeyValuePair<string, string>>();
            if (parameters != null)
                param.AddRange(parameters);

            param.Add(new KeyValuePair<string, string>("version", version));
            param.Add(new KeyValuePair<string, string>("wt", "xml"));

            return string.Join("&", param.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

        }
    }
}
