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

        private static readonly string[] AllFacetFields = new[] {"cat", "manu_exact"};

        public IEnumerable<string> SelectedFacetFields(SearchParameters parameters) {
            return parameters.Facets.Select(f => f.Key);
        }

        public ActionResult Index(SearchParameters parameters) {
            var start = (parameters.PageIndex - 1)*parameters.PageSize;
            var matchingProducts = solr.Query(BuildQuery(parameters), new QueryOptions {
                Rows = parameters.PageSize,
                Start = start,
                FacetQueries = AllFacetFields.Except(SelectedFacetFields(parameters))
                    .Select(f => new SolrFacetFieldQuery(f) {MinCount = 1})
                    .Cast<ISolrFacetQuery>()
                    .ToList(),
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