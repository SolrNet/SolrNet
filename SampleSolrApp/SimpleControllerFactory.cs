using System.Web.Mvc;
using System.Web.Routing;
using SolrNet.Utils;

namespace SampleSolrApp {
    public class SimpleControllerFactory : IControllerFactory {
        private readonly IContainer container;

        public SimpleControllerFactory(IContainer container) {
            this.container = container;
        }

        public IController CreateController(RequestContext requestContext, string controllerName) {
            return container.GetInstance<IController>(controllerName);
        }

        public void ReleaseController(IController controller) {}
    }
}