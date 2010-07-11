﻿#region license
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
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Impl;
using SolrNet.Impl.FieldParsers;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class FieldParserTests {
        [Test]
        public void FloatFieldParser_Parse() {
            var p = new FloatFieldParser();
            var v = p.Parse(XmlUtils.CreateNode("int", "31"), null);
            Assert.IsInstanceOfType(typeof(float), v);
            Assert.AreEqual(31f, v);
        }

        [Test]
        public void FloatFieldParser_cant_handle_string() {
            var p = new FloatFieldParser();
            Assert.Throws<FormatException>(() => p.Parse(XmlUtils.CreateNode("str", "pepe"), null));
        }

        public CollectionFieldParser CreateCollectionFieldParser() {
            var mocks = new MockRepository();
            var vp = mocks.StrictMock<ISolrFieldParser>();
            var p = new CollectionFieldParser(vp);
            return p;
        }

        [Test]
        [Row(typeof(string))]
        [Row(typeof(Dictionary<,>))]
        [Row(typeof(IDictionary<,>))]
        [Row(typeof(IDictionary<int, int>))]
        [Row(typeof(IDictionary))]
        [Row(typeof(Hashtable))]
        public void CollectionFieldParser_cant_handle_types(Type t) {
            var p = CreateCollectionFieldParser();
            Assert.IsFalse(p.CanHandleType(t));
        }

        [Test]
        [Row(typeof(IEnumerable))]
        [Row(typeof(IEnumerable<>))]
        [Row(typeof(IEnumerable<int>))]
        [Row(typeof(ICollection))]
        [Row(typeof(ICollection<>))]
        [Row(typeof(ICollection<int>))]
        [Row(typeof(IList))]
        [Row(typeof(IList<>))]
        [Row(typeof(IList<int>))]
        [Row(typeof(ArrayList))]
        [Row(typeof(List<>))]
        [Row(typeof(List<int>))]
        public void CollectionFieldParser_can_handle_types(Type t) {
            var p = CreateCollectionFieldParser();
            Assert.IsTrue(p.CanHandleType(t));
        }

        [Test]
        public void DoubleFieldParser() {
            var p = new DoubleFieldParser();
            p.Parse(XmlUtils.CreateNode("item", "123.99"), typeof(float));
        }

        [Test]
        public void DefaultFieldParser_EnumAsString() {
            var p = new DefaultFieldParser();
            var r = p.Parse(XmlUtils.CreateNode("str", "One"), typeof(Numbers));
            Assert.IsInstanceOfType(typeof(Numbers), r);
        }

        [Test]
        public void EnumAsString() {
            var p = new EnumFieldParser();
            var r = p.Parse(XmlUtils.CreateNode("str", "One"), typeof(Numbers));
            Assert.IsInstanceOfType(typeof(Numbers), r);
        }

        private enum Numbers {
            One, Two
        }

        [Test]
        public void SupportGuid() {
            var p = new DefaultFieldParser();
            var g = Guid.NewGuid();
            var r = p.Parse(XmlUtils.CreateNode("str", g.ToString()), typeof(Guid));
            var pg = (Guid)r;
            Assert.AreEqual(g, pg);
        }
    }
}