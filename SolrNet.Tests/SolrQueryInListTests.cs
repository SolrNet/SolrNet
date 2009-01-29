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

using NUnit.Framework;
using System.Linq;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryInListTests {
		[Test]
		public void ListOfInt() {
			var q = new SolrQueryInList("id", new[] {1, 2, 3, 4}.Select(i => i.ToString()));
			Assert.AreEqual("(id:1 OR id:2 OR id:3 OR id:4)", q.Query);
		}

        [Test]
        public void ShouldQuoteValues() {
            var q = new SolrQueryInList("id", new[] {"one", "two thousand"});
            Assert.AreEqual("(id:one OR id:\"two thousand\")", q.Query);
        }
	}
}