using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleSolrApp.Helpers {
    public static class UrlHelperExtensions {
        public static string SetParameter(this UrlHelper helper, string url, string key, string value) {
            var parts = url.Split('?');
            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1)
                qs = ParseQueryString(parts[1]);
            qs[key] = value;
            return parts[0] + "?" + DictToQuerystring(qs);
        }

        public static string DictToQuerystring(IDictionary<string, string> qs) {
            return string.Join("&", qs.Select(k => string.Format("{0}={1}", HttpUtility.UrlEncode(k.Key), HttpUtility.UrlEncode(k.Value))).ToArray());
        }

        public static string SetParameter(this UrlHelper helper, string key, string value) {
            return helper.SetParameter(helper.RequestContext.HttpContext.Request.RawUrl, key, value);
        }

        public static IDictionary<string, string> ParseQueryString(string s) {
            var d = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (s == null)
                return d;
            if (s.StartsWith("?"))
                s = s.Substring(1);
            foreach (var kv in s.Split('&')) {
                var v = kv.Split('=');
                if (string.IsNullOrEmpty(v[0]))
                    continue;
                d[HttpUtility.UrlDecode(v[0])] = HttpUtility.UrlDecode(v[1]);
            }
            return d;
        }
    }
}