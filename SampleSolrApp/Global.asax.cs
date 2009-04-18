#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using SampleSolrApp.Models;
using SampleSolrApp.Models.Binders;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

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
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Server.MapPath("/"), "log4net.config")));

            RegisterRoutes(RouteTable.Routes);
            var serverURL = "http://localhost:8983/solr";

            var connection = new SolrConnection(serverURL);
            var loggingConnection = new LoggingConnection(connection);
            Startup.Init<Product>(loggingConnection);

            RegisterAllControllers();
            ControllerBuilder.Current.SetControllerFactory(new ServiceProviderControllerFactory(Startup.Container));
            ModelBinders.Binders[typeof (SearchParameters)] = new SearchParametersBinder();
            AddInitialDocuments();
        }

        /// <summary>
        /// Adds some sample documents to Solr
        /// </summary>
        private void AddInitialDocuments() {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Delete(SolrQuery.All);
            var connection = ServiceLocator.Current.GetInstance<ISolrConnection>();
            foreach (var file in Directory.GetFiles(Server.MapPath("/exampledocs"), "*.xml")) {
                connection.Post("/update", File.ReadAllText(file, Encoding.UTF8));
            }
            solr.Commit();
            solr.BuildSpellCheckDictionary();
        }

        /// <summary>
        /// Gets a controller instance with its dependencies injected.
        /// Picks the first constructor on the controller.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="t">Controller type</param>
        /// <returns></returns>
        public IController GetContainerRegistration(IServiceProvider container, Type t) {
            var constructor = t.GetConstructors()[0];
            var dependencies = constructor.GetParameters().Select(p => container.GetService(p.ParameterType)).ToArray();
            return (IController) constructor.Invoke(dependencies);
        }

        public string GetControllerName(Type t) {
            return Regex.Replace(t.Name, "controller$", "", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Registers controllers in the DI container
        /// </summary>
        public void RegisterAllControllers() {
            var controllers = typeof (MvcApplication).Assembly.GetTypes().Where(t => typeof (IController).IsAssignableFrom(t));
            foreach (var controller in controllers) {
                Type controllerType = controller;
                Startup.Container.Register(GetControllerName(controller), controller, c => GetContainerRegistration(c, controllerType));
            }
        }
    }
}