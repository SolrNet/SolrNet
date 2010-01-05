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

using MbUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryByFieldTests {
		public class TestDocument {}

        [Test]
        public void NullField_yields_null_query() {
            var q = new SolrQueryByField(null, "123456");
            Assert.IsNull(q.Query);
        }

        [Test]
        public void NullValue_yields_null_query() {
            var q = new SolrQueryByField("id", null);
            Assert.IsNull(q.Query);
        }

		[Test]
		public void Basic() {
			var q = new SolrQueryByField("id", "123456");
			Assert.AreEqual("id:123456", q.Query);
		}

		[Test]
		public void ShouldQuoteSpaces() {
			var q = new SolrQueryByField("id", "hello world");
			Assert.AreEqual("id:\"hello world\"", q.Query);
		}

		[Test]
		public void ShouldQuoteSpecialChar() {
			var q = new SolrQueryByField("id", "hello+world-bye&&q||w!e(r)t{y}[u]^i\"o~p:a\\s+d;;");
			Assert.AreEqual(@"id:hello\+world\-bye\&&q\||w\!e\(r\)t\{y\}\[u\]\^i\""o\~p\:a\\s\+d\;\;", q.Query);
		}

		[Test]
		public void ShouldNotQuoteWildcard() {
			var q = new SolrQueryByField("id", "h?llo*");
			Assert.AreEqual("id:h?llo*", q.Query);
		}
	}
}