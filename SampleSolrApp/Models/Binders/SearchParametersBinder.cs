using System.Web.Mvc;

namespace SampleSolrApp.Models.Binders {
    public class SearchParametersBinder : IModelBinder {
        public const int DefaultPageSize = 5;

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var qs = controllerContext.HttpContext.Request.QueryString;
            var sp = new SearchParameters {
                FreeSearch = qs["q"],
                PageIndex = TryParse(qs["page"], 1),
                PageSize = TryParse(qs["pageSize"], DefaultPageSize),
            };
            return sp;
        }

        public int TryParse(string u, int defaultValue) {
            try {
                return int.Parse(u);
            } catch {
                return defaultValue;
            }
        }

        public int TryParse(string u) {
            return TryParse(u, 0);
        }
    }
}