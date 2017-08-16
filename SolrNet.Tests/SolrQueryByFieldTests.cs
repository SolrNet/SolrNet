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

using Xunit;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using System;

namespace SolrNet.Tests {
	
	public class SolrQueryByFieldTests {
		public class TestDocument {}

        public string Serialize(object q) {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }


        [Fact]
        public void NullField_yields_null_query() {
            var q = new SolrQueryByField(null, "123456");
            Assert.Null(Serialize(q));
        }

        [Fact]
        public void NullValue_yields_null_query() {
            var q = new SolrQueryByField("id", null);
            Assert.Null(Serialize(q));
        }

        [Fact]
        public void EmptyValue() {
            var q = new SolrQueryByField("id", "");
            Assert.Equal("id:(\"\")", Serialize(q));
        }

		[Fact]
		public void Basic() {
			var q = new SolrQueryByField("id", "123456");
            Assert.Equal("id:(123456)", Serialize(q));
		}

		[Fact]
		public void ShouldQuoteSpaces() {
			var q = new SolrQueryByField("id", "hello world");
            Assert.Equal("id:(\"hello world\")", Serialize(q));
		}

		[Fact]
		public void ShouldQuoteSpecialChar() {
			var q = new SolrQueryByField("id", "hello+world-bye&&q||w!e(r)t{y}[u]^i\"o~p:a\\s+d;;?*/");
            Assert.Equal(@"id:(hello\+world\-bye\&&q\||w\!e\(r\)t\{y\}\[u\]\^i\""o\~p\:a\\s\+d\;\;\?\*\/)", Serialize(q));
		}

        [Fact]
        public void QuotedFalse() {
            var q = new SolrQueryByField("id", "hello?wor/ld*") { Quoted = false };
            Assert.Equal("id:(hello?wor/ld*)", Serialize(q));
        }
	}
}