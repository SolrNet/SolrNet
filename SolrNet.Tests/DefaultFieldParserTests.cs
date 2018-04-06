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
using System.Xml.Linq;
using Xunit;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests {
    
    public class DefaultFieldParserTests {
        [Theory]
        [InlineData("str")]
        [InlineData("bool")]
        [InlineData("int")]
        [InlineData("date")]
        public void CanHandleSolrTypes(string solrType) {
            var p = new DefaultFieldParser();
            Assert.True(p.CanHandleSolrType(solrType));
        }

        [Theory]
        [InlineData(typeof(float))]
        [InlineData(typeof(float?))]
        [InlineData(typeof(double))]
        [InlineData(typeof(double?))]
        [InlineData(typeof(string))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTime?))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(DateTimeOffset?))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(bool?))]
        [InlineData(typeof(Money))]
        [InlineData(typeof(Location))]
        public void CanHandleType(Type t) {
            var p = new DefaultFieldParser();
            Assert.True(p.CanHandleType(t));
        }

        [Fact]
        public void ParseNullableInt() {
            var doc = new XDocument();
            doc.Add(new XElement("int", "31"));
            var p = new DefaultFieldParser();
            var i = p.Parse(doc.Root, typeof (int?));
            Assert.IsAssignableFrom<int?>(i);
            Assert.IsType<int>(i);
            var ii = (int?) i;
            Assert.True(ii.HasValue);
            Assert.Equal(31, ii.Value);
        }

        [Fact]
        public void ParseLocation() {
            var doc = new XDocument();
            doc.Add(new XElement("str", "31.2,-44.2"));
            var p = new DefaultFieldParser();
            var l = p.Parse(doc.Root, typeof(Location));
            Assert.IsType<Location>(l);
        }
    }
}