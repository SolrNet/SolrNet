using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class TypeHelperTests {
        [Test]
        public void IsGenericAssignableFrom() {
            Assert.IsTrue(TypeHelper.IsGenericAssignableFrom(typeof (IDictionary<,>), typeof (Dictionary<string, string>)));
        }

        [Test]
        public void IsNotGenericAssignableFrom() {
            Assert.IsFalse(TypeHelper.IsGenericAssignableFrom(typeof(IDictionary<,>), typeof(List<string>)));
        }

        [Test]
        public void IsNotGenericAssignableFrom_non_generic() {
            Assert.IsFalse(TypeHelper.IsGenericAssignableFrom(typeof(IList), typeof(List<string>)));
        }

        [Test]
        public void IsNullableType() {
            Assert.IsTrue(TypeHelper.IsNullableType(typeof (int?)));
        }

        [Test]
        public void IsNotNullableType() {
            Assert.IsFalse(TypeHelper.IsNullableType(typeof(int)));
        }

    }
}