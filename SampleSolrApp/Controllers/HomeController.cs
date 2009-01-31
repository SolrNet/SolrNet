using System.Web.Mvc;
using SampleSolrApp.Models;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace SampleSolrApp.Controllers {
    [HandleError]
    public class HomeController : Controller {
        private readonly ISolrReadOnlyOperations<Product> solr;

        public HomeController(ISolrReadOnlyOperations<Product> solr) {
            this.solr = solr;
        }

        public ActionResult Index(SearchParameters parameters) {
            var start = parameters.PageIndex * parameters.PageSize;
            var matchingProducts = solr.Query(parameters.FreeSearch ?? SolrQuery.All.Query, new QueryOptions {
                Rows = parameters.PageSize,
                Start = start,
            });
            return View(new ProductView {
                Products = matchingProducts,
                FirstResultIndex = start+1,
                LastResultIndex = start + parameters.PageSize+1,
                TotalCount = matchingProducts.NumFound,
            });
        }
    }
}