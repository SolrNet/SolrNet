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
using System.Collections.Generic;
using System.Linq;

namespace SampleSolrApp.Models {
    public class PaginationInfo {
        public PaginationInfo() {
            PageSlide = 2;
        }

        public int CurrentPage { get; set; }
        public int TotalItemCount { get; set; }
        public int PageSize { get; set; }
        public int PageSlide { get; set; }

        public IEnumerable<int> Pages {
            get {
                var pageCount = LastPage;
                var pageFrom = Math.Max(1, CurrentPage - PageSlide);
                var pageTo = Math.Min(pageCount - 1, CurrentPage + PageSlide);
                pageFrom = Math.Max(1, Math.Min(pageTo - 2*PageSlide, pageFrom));
                pageTo = Math.Min(pageCount, Math.Max(pageFrom + 2*PageSlide, pageTo));
                return Enumerable.Range(pageFrom, pageTo-pageFrom+1);
            }
        }

        public int LastPage {
            get {
                return (int)Math.Floor(((decimal)TotalItemCount - 1) / PageSize) + 1;
            }
        }

        public bool HasNextPage {
            get {
                return CurrentPage < LastPage;
            }
        }

        public string NextPageUrl {
            get {
                return HasNextPage ? PageUrlFor(CurrentPage + 1) : null;
            }
        }

        public bool HasPrevPage {
            get {
                return CurrentPage > 1;
            }
        }

        public string PrevPageUrl {
            get {
                return HasPrevPage ? PageUrlFor(CurrentPage - 1) : null;
            }
        }

        public string PageUrlFor(int page) {
            return PageUrl.Replace("!0", page.ToString());
        }

        public string PageUrl { get; set; }

        public int FirstItemIndex {
            get {
                return PageSize*(CurrentPage-1)+1;
            }
        }

        public int LastItemIndex {
            get {
                return Math.Min(FirstItemIndex + PageSize-1, TotalItemCount);
            }
        }
    }
}