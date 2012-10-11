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
using System.Xml;
using System.Xml.Linq;
using MbUnit.Framework;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests {
    [TestFixture]
    public class DefaultFieldParserTests {
        [Test]
        [Row("str")]
        [Row("bool")]
        [Row("int")]
        [Row("date")]
        public void CanHandleSolrTypes(string solrType) {
            var p = new DefaultFieldParser();
            Assert.IsTrue(p.CanHandleSolrType(solrType));
        }

        [Test]
        [Row(typeof(float))]
        [Row(typeof(float?))]
        [Row(typeof(double))]
        [Row(typeof(double?))]
        [Row(typeof(string))]
        [Row(typeof(DateTime))]
        [Row(typeof(DateTime?))]
        [Row(typeof(bool))]
        [Row(typeof(bool?))]
        [Row(typeof(Money))]
        [Row(typeof(Location))]
        public void CanHandleType(Type t) {
            var p = new DefaultFieldParser();
            Assert.IsTrue(p.CanHandleType(t));
        }

        [Test]
        public void ParseNullableInt() {
            var doc = new XDocument();
            doc.Add(new XElement("int", "31"));
            var p = new DefaultFieldParser();
            var i = p.Parse(doc.Root, typeof (int?));
            Assert.IsInstanceOfType(typeof(int?), i);
            var ii = (int?) i;
            Assert.IsTrue(ii.HasValue);
            Assert.AreEqual(31, ii.Value);
        }

        [Test]
        public void ParseLocation() {
            var doc = new XDocument();
            doc.Add(new XElement("str", "31.2,-44.2"));
            var p = new DefaultFieldParser();
            var l = p.Parse(doc.Root, typeof(Location));
            Assert.IsInstanceOfType<Location>(l);
        }
    }
}