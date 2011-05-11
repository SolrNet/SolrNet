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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Mapping;

namespace SolrNet.Tests {
    [TestFixture]
    public class MemoizingMappingManagerTests {
        [Test]
        public void CallsInnerJustOnce() {
            var mocks = new MockRepository();
            var innerMapper = mocks.StrictMock<IReadOnlyMappingManager>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(innerMapper.GetFields(typeof (TestDocument)))
                                     .Repeat.Once()
                                     .Return(new Dictionary<string,SolrFieldModel> {
                                         {"id", new SolrFieldModel { Property = typeof (TestDocument).GetProperty("Id"), FieldName = "id"}},
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
            var innerMapper = mocks.StrictMock<IReadOnlyMappingManager>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(innerMapper.GetUniqueKey(typeof (TestDocument)))
                                     .Repeat.Once()
                                     .Return(new SolrFieldModel { Property = typeof(TestDocument).GetProperty("Id"), FieldName = "id" }))
                .Verify(() => {
                    var mapper = new MemoizingMappingManager(innerMapper);
                    mapper.GetUniqueKey(typeof (TestDocument));
                    mapper.GetUniqueKey(typeof(TestDocument));
                });
        }

        [Test]
        public void GetRegistered() {
            var innerMapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            innerMapper.Expect(x => x.GetRegisteredTypes()).Repeat.Once().Return(new[] { typeof(TestDocument) });
            var mapper = new MemoizingMappingManager(innerMapper);
            var types = mapper.GetRegisteredTypes();
            Assert.AreEqual(1, types.Count);
            Assert.AreEqual(typeof (TestDocument), types.First());
            types = mapper.GetRegisteredTypes();
            innerMapper.VerifyAllExpectations();
        }

        public class TestDocument {
            public int Id { get; set; }
        }
    }
}