using System;
using System.Linq;
using NUnit.Framework;
using SampleSolrApp.Helpers;

namespace SampleSolrApp.Tests {
    [TestFixture]
    public class ForeachWithSeparatorTests {
        [Test]
        public void Elements() {
            var r = HtmlHelperRepeatExtensions.RepeatF(null, Enumerable.Range(0, 5), i => i.ToString(), () => " | ");
            Assert.AreEqual("0 | 1 | 2 | 3 | 4", r);
        }

        [Test]
        public void Empty() {
            var r = HtmlHelperRepeatExtensions.RepeatF(null, Enumerable.Range(0, 0), i => i.ToString(), () => " | ");
            Assert.AreEqual("", r);
        }

        [Test]
        public void ElementsAction() {
            HtmlHelperRepeatExtensions.Repeat(null, Enumerable.Range(0, 5), Console.Write, () => Console.Write(" | "));
        }
    }
}