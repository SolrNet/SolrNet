#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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
            With.Mocks(mocks).Expecting(() => Expect.Call(innerMapper.GetFields(typeof (TestDocument)))
                                                  .Repeat.Once()
                                                  .Return(new Dictionary<PropertyInfo, string> {
                                                      {typeof (TestDocument).GetProperty("Id"), "id"},
                                                  })).Verify(() => {
                                                      var mapper = new MemoizingMappingManager(innerMapper);
                                                      mapper.GetFields(typeof (TestDocument));
                                                      mapper.GetFields(typeof (TestDocument));
                                                  });
        }

        public class TestDocument {
            public int Id { get; set; }
        }
    }
}