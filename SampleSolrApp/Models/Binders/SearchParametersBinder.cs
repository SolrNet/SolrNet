using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using SampleSolrApp.Helpers;

namespace SampleSolrApp.Models.Binders {
    public class SearchParametersBinder : IModelBinder {
        public const int DefaultPageSize = 5;

        public bool StringEq(string a, string b) {
            return StringComparer.InvariantCultureIgnoreCase.Compare(a, b) == 0;
        }

        public IDictionary<string, string> NVToDict(NameValueCollection nv) {
            var d = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var k in nv.AllKeys)
                d[k] = nv[k];
            return d;
        }

        private static readonly Regex FacetRegex = new Regex("^f\\.", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var qs = controllerContext.HttpContext.Request.QueryString;
            var qsDict = NVToDict(qs);
            var sp = new SearchParameters {
                FreeSearch = StringHelper.EmptyToNull(qs["q"]),
                PageIndex = StringHelper.TryParse(qs["page"], 1),
                PageSize = StringHelper.TryParse(qs["pageSize"], DefaultPageSize),
                Facets = qsDict.Where(k => FacetRegex.IsMatch(k.Key))
                    .Select(k => k.WithKey(FacetRegex.Replace(k.Key, "")))
                    .ToDictionary()
            };
            return sp;
        }
    }
}