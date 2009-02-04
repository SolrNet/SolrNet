using System.Collections.Generic;
using System.Linq;
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

        public ISolrQuery BuildQuery(SearchParameters parameters) {
            var queriesFromFacets = from p in parameters.Facets
                                    let q = new SolrQueryByField(p.Key, p.Value)
                                    select q as ISolrQuery;
            var queries = new List<ISolrQuery>(queriesFromFacets);
            if (!string.IsNullOrEmpty(parameters.FreeSearch))
                queries.Add(new SolrQuery(parameters.FreeSearch));
            if (queries.Count == 0)
                return SolrQuery.All;
            return new SolrMultipleCriteriaQuery(queries, SolrMultipleCriteriaQuery.Operator.AND);
        }

        public ActionResult Index(SearchParameters parameters) {
            var start = (parameters.PageIndex - 1)*parameters.PageSize;
            var matchingProducts = solr.Query(BuildQuery(parameters), new QueryOptions {
                Rows = parameters.PageSize,
                Start = start,
                FacetQueries = new[] {new SolrFacetFieldQuery("cat"), }
            });
            var view = new ProductView {
                Products = matchingProducts,
                Search = parameters,
                TotalCount = matchingProducts.NumFound,
                Facets = matchingProducts.FacetFields,
            };
            return View(view);
        }
    }
}