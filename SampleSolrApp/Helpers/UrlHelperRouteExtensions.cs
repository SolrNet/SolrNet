using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace SampleSolrApp.Helpers {
    public static class UrlHelperRouteExtensions {
        public static string SetRouteParameters(this UrlHelper helper, IDictionary<string, object> parameters) {
            var routeParams = new RouteValueDictionary(helper.RequestContext.RouteData.Values);
            foreach (var p in parameters)
                routeParams[p.Key] = p.Value;
            return helper.RouteUrl(routeParams);
        }

        public static string SetRouteParameters(this UrlHelper helper, object parameters) {
            return helper.SetRouteParameters(parameters.ToPropertyDictionary());
        }

        public static string SetRouteParameter(this UrlHelper helper, string key, object value) {
            return helper.SetRouteParameters(new Dictionary<string, object> {
                {key, value},
            });
        }
    }
}