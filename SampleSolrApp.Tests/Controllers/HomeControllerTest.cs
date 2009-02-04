using NUnit.Framework;
using Rhino.Mocks;
using SampleSolrApp.Controllers;
using SampleSolrApp.Models;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace SampleSolrApp.Tests.Controllers {
    [TestFixture]
    public class HomeControllerTest {
        [Test]
        public void Index_Without_parameters() {
            var solr = MockRepository.GenerateMock<ISolrReadOnlyOperations<Product>>();
            solr.Expect(o => o.Query("*:*", new QueryOptions()))
                .IgnoreArguments()
                .Return(new SolrQueryResults<Product>());
            var c = new HomeController(solr);
            var result = c.Index(new SearchParameters());   
        }

        [Test]
        public void Facets() {
            
        }
    }
}