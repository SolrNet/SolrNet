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
using MbUnit.Framework;
using SolrNet.Impl.FieldSerializers;

namespace SolrNet.Tests {
    [TestFixture]
    public class GenericDictionaryFieldSerializerTests {
        [Test]
        public void Serialize_null_returns_empty() {
            var s = new GenericDictionaryFieldSerializer(new DefaultFieldSerializer());
            var l = s.Serialize(null).ToList();
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void Serialize_dict_with_null_elements() {
            var s = new GenericDictionaryFieldSerializer(new DefaultFieldSerializer());
            var dict = new Dictionary<string, string> {
                {"one","1"},
                {"two",null},
            };
            var l = s.Serialize(dict).ToList();
            Assert.AreEqual(2, l.Count);
        }
    }
}