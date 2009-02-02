using System.Linq;
using NUnit.Framework;
using SampleSolrApp.Models;

namespace SampleSolrApp.Tests {
    [TestFixture]
    public class PaginationTests {
        [Test]
        public void tt() {
            var info = new PaginationInfo {
                CurrentPage = 8,
                PageSlide = 4,
                PageSize = 5,
                TotalItemCount = 500,
            };

            var pages = info.Pages.ToArray();
            Assert.AreEqual(9, pages.Length);
            Assert.AreEqual(4, pages[0]);
            Assert.AreEqual(12, pages.Last());
        }

        [Test]
        public void tt2() {
            var info = new PaginationInfo {
                CurrentPage = 1,
                PageSlide = 2,
                PageSize = 5,
                TotalItemCount = 25,
            };

            var pages = info.Pages.ToArray();
            Assert.AreEqual(5, pages.Length);
            Assert.AreEqual(1, pages[0]);
            Assert.AreEqual(5, pages.Last());
        }
    }
}