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

using System.Collections.Generic;

namespace SampleSolrApp.Models {
    public class SearchParameters {
        public const int DefaultPageSize = 5;

        public SearchParameters() {
            Facets = new Dictionary<string, string>();
            PageSize = DefaultPageSize;
            PageIndex = 1;
        }

        public string FreeSearch { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IDictionary<string, string> Facets { get; set; }
        public string Sort { get; set; }

        public int FirstItemIndex {
            get {
                return (PageIndex-1)*PageSize;
            }
        }

        public int LastItemIndex {
            get {
                return FirstItemIndex + PageSize;
            }
        }
    }
}