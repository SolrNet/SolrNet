using System.Collections.Generic;
using System.Web.Mvc;

namespace SampleSolrApp.Helpers {
    public static class UrlHelperFacetExtensions {
        public static string SetFacet(this UrlHelper helper, string field, string value) {
            return helper.SetParameters(helper.RequestContext.HttpContext.Request.RawUrl, new Dictionary<string, object> {
                {string.Format("f_{0}", field), value},
                {"page", 1},
            });
        }
    }
}