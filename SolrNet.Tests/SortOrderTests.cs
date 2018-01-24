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
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	
	public class SortOrderTests {
		[Fact]
		public void Constructor_ShouldAcceptSpaces() {
            var o = new SortOrder("dist(2, point1, point2)", Order.DESC);
            Assert.Equal("dist(2, point1, point2) desc", o.ToString());
		}

		[Fact]
		public void DefaultOrder() {
			var o = new SortOrder("uno");
			Assert.Equal("uno asc", o.ToString());
		}

		[Fact]
		public void ParseNull_ShouldReturnNull() {
			var o = SortOrder.Parse(null);
            Assert.Null(o);
		}

        [Fact]
        public void ParseEmpty_ShouldReturnNull() {
            var o = SortOrder.Parse("");
            Assert.Null(o);
        }

		[Fact]
		public void Parse() {
			var o = SortOrder.Parse("pepe");
			Assert.Equal("pepe asc", o.ToString());
		}

		[Fact]
		public void ParseAsc() {
			var o = SortOrder.Parse("pepe asc");
            Assert.Equal("pepe asc", o.ToString(),true,true,true);
		}

		[Fact]
		public void ParseDesc() {
			var o = SortOrder.Parse("pepe desc");
            Assert.Equal("pepe desc", o.ToString(), true, true, true);
		}

        [Fact]
        public void ParseDescWithSpaces() {
            var o = SortOrder.Parse("pepe  desc");
            Assert.Equal("pepe desc", o.ToString(), true, true, true);
        }

		[Fact]
		public void InvalidParse_ShouldThrow() {
		Assert.Throws< InvalidSortOrderException>( () => SortOrder.Parse("pepe bla"));
		}

	    [Fact]
	    public void FieldName_accessor() {
            var o = SortOrder.Parse("pepe asc");
            Assert.Equal("pepe", o.FieldName);
	    }

        [Fact]
        public void Order_accessor()
        {
            var o = SortOrder.Parse("pepe asc");
            Assert.Equal(Order.ASC, o.Order);
        }

	    [Fact]
	    public void SortOrders_are_equal_if_field_name_and_order_are_equal() {
	        var sortOrder1 = new SortOrder("fieldName", Order.ASC);
	        var sortOrder2 = new SortOrder("fieldName", Order.ASC);
            Assert.Equal(sortOrder1, sortOrder2);
	    }

        [Fact]
        public void SortOrders_are_not_equal_if_field_name_is_different() {
            var sortOrder1 = new SortOrder("fieldName", Order.ASC);
            var sortOrder2 = new SortOrder("otherField", Order.ASC);
            Assert.NotEqual(sortOrder1, sortOrder2);
        }

        
        [Fact]
        public void SortOrders_are_not_equal_if_order_is_different() {
            var sortOrder1 = new SortOrder("fieldName", Order.ASC);
            var sortOrder2 = new SortOrder("fieldName", Order.DESC);
            Assert.NotEqual(sortOrder1, sortOrder2);
        }
    }
}