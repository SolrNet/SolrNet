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
    }
}