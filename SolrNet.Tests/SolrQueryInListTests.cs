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
using System.Linq;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryInListTests {

        public string Serialize(object q) {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }

		[Test]
		public void ListOfInt() {
			var q = new SolrQueryInList("id", new[] {1, 2, 3, 4}.Select(i => i.ToString()));
            Assert.AreEqual("(id:((1) OR (2) OR (3) OR (4)))", Serialize(q));
		}

        [Test]
        public void ShouldQuoteValues() {
            var q = new SolrQueryInList("id", new[] {"one", "two thousand"});
            Assert.AreEqual("(id:((one) OR (\"two thousand\")))", Serialize(q));
        }


        [Test]
        public void ShouldQuoteEmptyValues() {
            var q = new SolrQueryInList("id", new[] { "", "two thousand" });
            Assert.AreEqual("(id:((\"\") OR (\"two thousand\")))", Serialize(q));
        }

        [Test]
        public void EmptyList_should_be_null_query() {
            var q = new SolrQueryInList("id", new string[0]);
            Assert.IsNull(Serialize(q));
        }

        [Test]
        public void Fieldname_with_spaces()
        {
            var q = new SolrQueryInList("i have spaces", new[] { "one", "two thousand" });
            Assert.AreEqual("(i\\ have\\ spaces:((one) OR (\"two thousand\")))", Serialize(q));
        }
	}
}