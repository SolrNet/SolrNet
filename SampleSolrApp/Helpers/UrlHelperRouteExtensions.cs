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

using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace SampleSolrApp.Helpers {
    /// <summary>
    /// Route parameters processing extensions
    /// </summary>
    public static class UrlHelperRouteExtensions {
        /// <summary>
        /// Sets/changes the current route parameters
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parameters">Parameters to set/change</param>
        /// <returns>Resulting URL</returns>
        public static string SetRouteParameters(this UrlHelper helper, IDictionary<string, object> parameters) {
            var routeParams = new RouteValueDictionary(helper.RequestContext.RouteData.Values);
            foreach (var p in parameters)
                routeParams[p.Key] = p.Value;
            return helper.RouteUrl(routeParams);
        }

        /// <summary>
        /// Sets/changes the current route parameters
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="parameters"></param>
        /// <returns>Resulting URL</returns>
        public static string SetRouteParameters(this UrlHelper helper, object parameters) {
            return helper.SetRouteParameters(parameters.ToPropertyDictionary());
        }

        /// <summary>
        /// Sets/changes a single route parameter
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="key">Route param key</param>
        /// <param name="value">Route param value</param>
        /// <returns>Resulting URL</returns>
        public static string SetRouteParameter(this UrlHelper helper, string key, object value) {
            return helper.SetRouteParameters(new Dictionary<string, object> {
                {key, value},
            });
        }
    }
}