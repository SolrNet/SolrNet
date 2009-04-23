using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Impl;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests {
    [TestFixture]
    public class FieldParserTests {
        public XmlNode CreateNode(string name, string innerText) {
            var doc = new XmlDocument();
            var node = doc.CreateElement(name);
            node.InnerText = innerText;
            return node;
        }

        [Test]
        public void FloatFieldParser_Parse() {
            var p = new FloatFieldParser();
            var v = p.Parse(CreateNode("int", "31"), null);
            Assert.IsInstanceOfType(typeof(float), v);
            Assert.AreEqual(31f, v);
        }

        [Test]
        public void FloatFieldParser_cant_handle_string() {
            var p = new FloatFieldParser();
            Assert.Throws<FormatException>(() => p.Parse(CreateNode("str", "pepe"), null));
        }

        public CollectionFieldParser CreateCollectionFieldParser() {
            var mocks = new MockRepository();
            var vp = mocks.CreateMock<ISolrFieldParser>();
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
    }
}