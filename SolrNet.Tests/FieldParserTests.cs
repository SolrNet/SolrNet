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
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests {
    
    public class FieldParserTests {
        [Fact]
        public void FloatFieldParser_Parse() {
            var p = new FloatFieldParser();
            var xml = new XDocument();
            xml.Add(new XElement("int", "31"));
            var v = p.Parse(xml.Root, null);
            Assert.IsType<float>(v);
            Assert.Equal(31f, v);
        }

        [Fact]
        public void FloatFieldParser_cant_handle_string() {
            var p = new FloatFieldParser();
            var xml = new XDocument();
            xml.Add(new XElement("str", "pepe"));
            Assert.Throws<FormatException>(() => p.Parse(xml.Root, null));
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(Dictionary<,>))]
        [InlineData(typeof(IDictionary<,>))]
        [InlineData(typeof(IDictionary<int, int>))]
        [InlineData(typeof(IDictionary))]
        [InlineData(typeof(Hashtable))]
        public void CollectionFieldParser_cant_handle_types(Type t) {
            var p = new CollectionFieldParser(null);
            Assert.False(p.CanHandleType(t));
        }

        [Theory]
        [InlineData(typeof(IEnumerable))]
        [InlineData(typeof(IEnumerable<>))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(ICollection))]
        [InlineData(typeof(ICollection<>))]
        [InlineData(typeof(ICollection<int>))]
        [InlineData(typeof(IList))]
        [InlineData(typeof(IList<>))]
        [InlineData(typeof(IList<int>))]
        [InlineData(typeof(ArrayList))]
        [InlineData(typeof(List<>))]
        [InlineData(typeof(List<int>))]
        public void CollectionFieldParser_can_handle_types(Type t) {
            var p = new CollectionFieldParser(null);
            Assert.True(p.CanHandleType(t));
        }

        [Fact]
        public void DoubleFieldParser() {
            var p = new DoubleFieldParser();
            var xml = new XDocument();
            xml.Add(new XElement("item", "123.99"));
            p.Parse(xml.Root, typeof(float));
        }

        [Fact]
        public void DecimalFieldParser() {
            var p = new DecimalFieldParser();
            var xml = new XDocument();
            xml.Add(new XElement("item", "6.66E13"));
            var value = (decimal) p.Parse(xml.Root, typeof(decimal));
            Assert.Equal(66600000000000m, value);
        }

        [Fact]
        public void DecimalFieldParser_overflow() {
            var p = new DecimalFieldParser();
            var xml = new XDocument();
            xml.Add(new XElement("item", "6.66E53"));
            Assert.Throws<OverflowException>(() => (decimal)p.Parse(xml.Root, typeof(decimal)));
        }

        [Fact]
        public void DefaultFieldParser_EnumAsString() {
            var p = new DefaultFieldParser();
            var xml = new XDocument();
            xml.Add(new XElement("str", "One"));
            var r = p.Parse(xml.Root, typeof(Numbers));
            Assert.IsType<Numbers>(r);
        }

        [Fact]
        public void EnumAsString() {
            var p = new EnumFieldParser();
            var xml = new XDocument();
            xml.Add(new XElement("str", "One"));
            var r = p.Parse(xml.Root, typeof(Numbers));
            Assert.IsType<Numbers>(r);
        }

        private enum Numbers {
            One, Two
        }

        [Fact]
        public void SupportGuid() {
            var p = new DefaultFieldParser();
            var g = Guid.NewGuid();
            var xml = new XDocument();
            xml.Add(new XElement("str", g.ToString()));
            var r = p.Parse(xml.Root, typeof(Guid));
            var pg = (Guid)r;
            Assert.Equal(g, pg);
        }

        [Fact]
        public void SupportsNullableGuid() {
            var p = new DefaultFieldParser();
            var g = Guid.NewGuid();
            var xml = new XDocument();
            xml.Add(new XElement("str", g.ToString()));
            var r = p.Parse(xml.Root, typeof(Guid?));
            var pg = (Guid?)r;
            Assert.Equal(g, pg.Value);
        }
    }
}