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

using System;
using System.Linq;
using MbUnit.Framework;
using SolrNet.Attributes;
using SolrNet.Exceptions;
using SolrNet.Mapping;

namespace SolrNet.Tests {
	[TestFixture]
	public class AttributesMappingManagerTests {
		[Test]
		public void GetFields() {
			var m = new AttributesMappingManager();
			var fields = m.GetFields(typeof (Entity)).Values;
			Assert.AreEqual(2, fields.Count);
			foreach (var f in fields) {
				if (f.FieldName == "Id")
					Assert.AreEqual("Id", f.Property.Name);
				else if (f.FieldName == "desc")
					Assert.AreEqual("Description", f.Property.Name);
				else
					Assert.Fail("Invalid field '{0}'", f.FieldName);
			}
		}

		[Test]
		public void GetUniqueKey() {
			var m = new AttributesMappingManager();
			var key = m.GetUniqueKey(typeof (Entity));
			Assert.IsNotNull(key);
			Assert.IsNotNull(key.Property);
			Assert.AreEqual("Id", key.Property.Name);
			Assert.AreEqual("Id", key.FieldName);
		}

        [Test]
        public void DifferentTypes() {
            var m = new AttributesMappingManager();
            var key = m.GetUniqueKey(typeof(Entity));
            Assert.IsNotNull(key);
            Assert.IsNotNull(key.Property);
            Assert.AreEqual("Id", key.Property.Name);
            Assert.AreEqual("Id", key.FieldName);
            var fields = m.GetFields(typeof (AnotherEntity));
            Assert.AreEqual(1, fields.Count);
        }

        [Test]
        public void NoProperties_ShouldReturnEmpty() {
            var m = new AttributesMappingManager();
            var fields = m.GetFields(typeof (NoProperties));
            Assert.AreEqual(0, fields.Count);
        }

        [Test]
        public void GetUniqueKey_without_unique_key_throws() {
            var m = new AttributesMappingManager();
            var pk = m.GetUniqueKey(typeof (AnotherEntity));
            Assert.IsNull(pk);
        }

        [Test]
        public void Inherited() {
            var m = new AttributesMappingManager();
            var fields = m.GetFields(typeof (InheritedEntity));
            Assert.AreEqual(3, fields.Count);
            var uniqueKey = m.GetUniqueKey(typeof(InheritedEntity));
            Assert.IsNotNull(uniqueKey);
            Assert.AreEqual("Id", uniqueKey.FieldName);
        }

        [Test]
        public void GetRegisteredTypes() {
            var m = new AttributesMappingManager();
            var types = m.GetRegisteredTypes();
            Assert.GreaterThan(types.Count, 0);
            Assert.Contains(types, typeof(Entity));
            Assert.Contains(types, typeof(InheritedEntity));
            Assert.Contains(types, typeof(AnotherEntity));
            Assert.DoesNotContain(types, typeof(NoProperties));
        }

        public class NoProperties {}

		public class Entity {
			[SolrUniqueKey]
			public int Id { get; set; }

			[SolrField("desc")]
			public string Description { get; set; }
		}

        public class InheritedEntity: Entity {
            [SolrField("ts")]
            public DateTime Timestamp { get; set; }
        }

        public class AnotherEntity {
            [SolrField]
            public string Name { get; set; }
        }
	}
}