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
    public class CollectionFieldSerializerTests {
        [Test]
        public void Serialize_null_returns_empty() {
            var s = new CollectionFieldSerializer(new DefaultFieldSerializer());
            var p = s.Serialize(null).ToList();
            Assert.AreEqual(0, p.Count);
        }

        [Test]
        public void Serialize_collection_with_null_element() {
            var s = new CollectionFieldSerializer(new DefaultFieldSerializer());
            var c = new List<string> {"a", null };
            var p = s.Serialize(c).ToList();
            Assert.AreEqual(2, p.Count);
        }
    }
}