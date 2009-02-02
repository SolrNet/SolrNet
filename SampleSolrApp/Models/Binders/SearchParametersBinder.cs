using System.Web.Mvc;
using SampleSolrApp.Helpers;

namespace SampleSolrApp.Models.Binders {
    public class SearchParametersBinder : IModelBinder {
        public const int DefaultPageSize = 5;

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var qs = controllerContext.HttpContext.Request.QueryString;
            var sp = new SearchParameters {
                FreeSearch = StringHelper.EmptyToNull(qs["q"]),
                PageIndex = StringHelper.TryParse(qs["page"], 1),
                PageSize = StringHelper.TryParse(qs["pageSize"], DefaultPageSize),
            };
            return sp;
        }

    }
}