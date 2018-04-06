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
using Xunit;
using SolrNet.Mapping;
using SolrNet.Tests.Mocks;

namespace SolrNet.Tests {
    
    public class MemoizingMappingManagerTests {
        [Fact]
        public void CallsInnerJustOnce() {
            var innerMapper = new MReadOnlyMappingManager();
            innerMapper.getFields += t => {
                Assert.Equal(typeof (TestDocument), t);
                return new Dictionary<string, SolrFieldModel> {
                    {"id", new SolrFieldModel (property : typeof (TestDocument).GetProperty("Id"), fieldName : "id")},
                };
            };
            var mapper = new MemoizingMappingManager(innerMapper);
            mapper.GetFields(typeof (TestDocument));
            mapper.GetFields(typeof (TestDocument));
            Assert.Equal(1, innerMapper.getFields.Calls);
        }

        [Fact]
        public void GetUniqueKeyIsMemoized() {
            var innerMapper = new MReadOnlyMappingManager();
            innerMapper.getUniqueKey += t => {
	            Assert.Equal(typeof (TestDocument), t);
	            return new SolrFieldModel(property : typeof (TestDocument).GetProperty("Id"),
	                                      fieldName : "id");
            };
            var mapper = new MemoizingMappingManager(innerMapper);
            mapper.GetUniqueKey(typeof(TestDocument));
            mapper.GetUniqueKey(typeof(TestDocument));
            Assert.Equal(1, innerMapper.getUniqueKey.Calls);
        }

        [Fact]
        public void GetRegistered() {
            var innerMapper = new MReadOnlyMappingManager();
            innerMapper.getRegisteredTypes += () => new[] {typeof (TestDocument)};
            var mapper = new MemoizingMappingManager(innerMapper);
            var types = mapper.GetRegisteredTypes();
            Assert.Equal(1, types.Count);
            Assert.Equal(typeof (TestDocument), types.First());
            mapper.GetRegisteredTypes();
        }

        public class TestDocument {
            public int Id { get; set; }
        }
    }
}