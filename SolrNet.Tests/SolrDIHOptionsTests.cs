using MbUnit.Framework;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrDIHOptionsTests {
        [Test]
        public void DefaultValueTest() {
            var opt = new DIHOptions();
            Assert.AreEqual("dataimport", opt.HandlerName);
            Assert.IsFalse(opt.Clean.HasValue);
            Assert.IsFalse(opt.Debug.HasValue);
            Assert.IsFalse(opt.Commit.HasValue);
            Assert.IsFalse(opt.Optimize.HasValue);
        }

        [Test]
        public void HandlerNameConstructorTest() {
            var opt = new DIHOptions("custom-import");
            Assert.AreEqual("custom-import", opt.HandlerName);
            Assert.IsFalse(opt.Clean.HasValue);
            Assert.IsFalse(opt.Debug.HasValue);
            Assert.IsFalse(opt.Commit.HasValue);
            Assert.IsFalse(opt.Optimize.HasValue);
        }

        [Test]
        public void ToQueryParametersBasicTest() {
            var parameters = SolrDIHOperations.OptionsToParameters(new DIHOptions());
            Assert.IsEmpty(parameters);
        }

        [Test]
        public void ToQueryParametersWithAllTrueTest() {
            var parameters = new DIHOptions {
                Clean = true,
                Commit = true,
                Debug = true,
                Optimize = true
            };
            Assert.AreElementsEqualIgnoringOrder(new [] {
                KV.Create("clean", "true"),
                KV.Create("commit", "true"),
                KV.Create("debug", "true"),
                KV.Create("optimize", "true"),
            }, SolrDIHOperations.OptionsToParameters(parameters));
        }

        [Test]
        public void ToQueryParametersWithAllFalseTest() {
            var parameters = new DIHOptions {
                                 Clean = false,
                                 Commit = false,
                                 Debug = false,
                                 Optimize = false
                             };
            Assert.AreElementsEqualIgnoringOrder(new[] {
                KV.Create("clean", "false"),
                KV.Create("commit", "false"),
                KV.Create("debug", "false"),
                KV.Create("optimize", "false"),
            }, SolrDIHOperations.OptionsToParameters(parameters));
        }
    }
}