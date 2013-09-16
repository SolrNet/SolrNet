#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SampleSolrApp.Helpers {
    /// <summary>
    /// Query string processing extensions
    /// </summary>
    public static class UrlHelperExtensions {
        /// <summary>
        /// Sets/changes an url's query string parameter.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url">URL to process</param>
        /// <param name="key">Query string parameter key to set/change</param>
        /// <param name="value">Query string parameter value</param>
        /// <returns>Resulting URL</returns>
        public static string SetParameter(this UrlHelper helper, string url, string key, string value) {
            return helper.SetParameters(url, new Dictionary<string, object> {
                {key, value}
            });
        }

        /// <summary>
        /// Sets/changes an url's query string parameters.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url">URL to process</param>
        /// <param name="parameters">Paramteres to set/change</param>
        /// <returns>Resulting URL</returns>
        public static string SetParameters(this UrlHelper helper, string url, IDictionary<string, object> parameters) {
            var parts = url.Split('?');
            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1)
                qs = ParseQueryString(parts[1]);
            foreach (var p in parameters)
                qs[p.Key] = p.Value.ToNullOrString();
            return parts[0] + "?" + DictToQuerystring(qs);
        }

        /// <summary>
        /// Removes parameters from an url's query string
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url">URL to process</param>
        /// <param name="parameters">Query string parameter keys to remove</param>
        /// <returns>Resulting URL</returns>
        public static string RemoveParametersUrl(this UrlHelper helper, string url, params string[] parameters) {
            var parts = url.Split('?');
            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1)
                qs = ParseQueryString(parts[1]);
            foreach (var p in parameters)
                qs.Remove(p);
            return parts[0] + "?" + DictToQuerystring(qs);
        }

        public static string RemoveParameters(this UrlHelper helper, params string[] parameters) {
            return helper.RemoveParametersUrl(helper.RequestContext.HttpContext.Request.RawUrl, parameters);
        }

        public static string DictToQuerystring(IDictionary<string, string> qs) {
            return string.Join("&", qs
                .Where(k => !string.IsNullOrEmpty(k.Key))
                .Select(k => string.Format("{0}={1}", HttpUtility.UrlEncode(k.Key), HttpUtility.UrlEncode(k.Value))).ToArray());
        }

        /// <summary>
        /// Sets/changes a single parameter from the current query string.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="key">Parameter key</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Resulting URL</returns>
        public static string SetParameter(this UrlHelper helper, string key, object value) {
            return helper.SetParameter(helper.RequestContext.HttpContext.Request.RawUrl, key, value.ToNullOrString());
        }

        /// <summary>
        /// Sets/changes the current query string's parameters, using <paramref name="parameterDictionary"/> as dictionary
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parameterDictionary">Parameters to set/change</param>
        /// <returns>Resulting URL</returns>
        public static string SetParameters(this UrlHelper helper, object parameterDictionary) {
            return helper.SetParameters(helper.RequestContext.HttpContext.Request.RawUrl, parameterDictionary.ToPropertyDictionary());
        }

        /// <summary>
        /// Parses a query string. If duplicates are present, the last key/value is kept.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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

        public static string ForQuery(this UrlHelper helper, string solrQuery) {
            return helper.SetParameter("q", solrQuery);
        }
    }
}