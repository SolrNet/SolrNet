using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.ServiceLocation;
using SampleSolrApp.Models;
using SampleSolrApp.Models.Binders;
using SolrNet;
using SolrNet.Utils;

namespace SampleSolrApp {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}", // URL with parameters
                new {controller = "Home", action = "Index"} // Parameter defaults
                );
        }

        protected void Application_Start() {
            RegisterRoutes(RouteTable.Routes);
            Startup.Init<Product>("http://localhost:8983/solr");
            RegisterAllControllers();
            ControllerBuilder.Current.SetControllerFactory(new SimpleControllerFactory(Startup.Container));
            ModelBinders.Binders[typeof (SearchParameters)] = new SearchParametersBinder();
            AddInitialDocuments();
        }

        private void AddInitialDocuments() {
            var connection = ServiceLocator.Current.GetInstance<ISolrConnection>();
            foreach (var file in Directory.GetFiles(Server.MapPath("/exampledocs"), "*.xml")) {
                connection.Post("/update", File.ReadAllText(file, Encoding.UTF8));
            }
            ServiceLocator.Current.GetInstance<ISolrOperations<Product>>().Commit();
        }

        public IController GetContainerRegistration(IContainer container, Type t) {
            var constructor = t.GetConstructors()[0];
            var dependencies = constructor.GetParameters().Select(p => container.GetInstance(p.ParameterType)).ToArray();
            return (IController) constructor.Invoke(dependencies);
        }

        public string GetControllerName(Type t) {
            return Regex.Replace(t.Name, "controller$", "", RegexOptions.IgnoreCase);
        }

        public void RegisterAllControllers() {
            var controllers = typeof (MvcApplication).Assembly.GetTypes().Where(t => typeof (IController).IsAssignableFrom(t));
            foreach (var controller in controllers)
                Startup.Container.Register(GetControllerName(controller), c => GetContainerRegistration(c, controller));
        }
    }
}