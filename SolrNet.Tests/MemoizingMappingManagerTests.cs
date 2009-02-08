using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;

namespace SolrNet.Tests {
    [TestFixture]
    public class MemoizingMappingManagerTests {
        [Test]
        public void CallsInnerJustOnce() {
            var mocks = new MockRepository();
            var innerMapper = mocks.CreateMock<IReadOnlyMappingManager>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(innerMapper.GetFields(typeof (TestDocument)))
                                     .Repeat.Once()
                                     .Return(new Dictionary<PropertyInfo, string> {
                                         {typeof (TestDocument).GetProperty("Id"), "id"},
                                     }))
                .Verify(() => {
                    var mapper = new MemoizingMappingManager(innerMapper);
                    mapper.GetFields(typeof (TestDocument));
                    mapper.GetFields(typeof (TestDocument));
                });
        }

        [Test]
        public void GetUniqueKeyIsMemoized() {
            var mocks = new MockRepository();
            var innerMapper = mocks.CreateMock<IReadOnlyMappingManager>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(innerMapper.GetUniqueKey(typeof (TestDocument)))
                                     .Repeat.Once()
                                     .Return(new KeyValuePair<PropertyInfo, string>(typeof(TestDocument).GetProperty("Id"), "id")))
                .Verify(() => {
                    var mapper = new MemoizingMappingManager(innerMapper);
                    mapper.GetUniqueKey(typeof (TestDocument));
                    mapper.GetUniqueKey(typeof(TestDocument));
                });
        }

        public class TestDocument {
            public int Id { get; set; }
        }
    }
}