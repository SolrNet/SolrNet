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

using System.Collections.Generic;
using System.Web.Mvc;

namespace SampleSolrApp.Helpers {
    public static class UrlHelperFacetExtensions {
        public static string SetFacet(this UrlHelper helper, string field, string value) {
            return helper.SetParameters(helper.RequestContext.HttpContext.Request.RawUrl, new Dictionary<string, object> {
                {string.Format("f_{0}", field), value},
                {"page", 1},
            });
        }

        public static string RemoveFacet(this UrlHelper helper, string field) {
            var noFacet = helper.RemoveParametersUrl(helper.RequestContext.HttpContext.Request.RawUrl, string.Format("f_{0}", field));
            return helper.SetParameter(noFacet, "page", "1");
        }
    }
}