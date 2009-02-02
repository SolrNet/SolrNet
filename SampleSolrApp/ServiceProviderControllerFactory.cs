using System;
using System.Web;
using System.Web.Mvc;

namespace SampleSolrApp {
    public class ServiceProviderControllerFactory : DefaultControllerFactory {
        private readonly IServiceProvider container;

        public ServiceProviderControllerFactory(IServiceProvider container) {
            this.container = container;
        }

        protected override IController GetControllerInstance(Type controllerType) {
            if (controllerType == null) {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", RequestContext.HttpContext.Request.Path));
            }

            return (IController) container.GetService(controllerType);
        }

        public override void ReleaseController(IController controller) {
            var disposable = controller as IDisposable;

            if (disposable != null) {
                disposable.Dispose();
            }
        }
    }
}