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

using System.Linq;
using MbUnit.Framework;
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