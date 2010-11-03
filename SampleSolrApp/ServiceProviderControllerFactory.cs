#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SampleSolrApp {
    public class ServiceProviderControllerFactory : DefaultControllerFactory {
        private readonly IServiceProvider container;

        public ServiceProviderControllerFactory(IServiceProvider container) {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext context, Type controllerType) {
            if (controllerType == null) {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", context.HttpContext.Request.Path));
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