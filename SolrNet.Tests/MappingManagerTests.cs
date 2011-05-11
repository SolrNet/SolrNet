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
using SolrNet.Mapping;

namespace SolrNet.Tests {
    [TestFixture]
    public class MappingManagerTests {
        [Test]
        public void AddAndGet() {
            var mgr = new MappingManager();
            mgr.Add(typeof (Entity).GetProperty("Id"), "id");
            var fields = mgr.GetFields(typeof (Entity));
            Assert.AreEqual(1, fields.Count);
        }

        [Test]
        public void No_Mapped_type_returns_empty() {
            var mgr = new MappingManager();
            var fields = mgr.GetFields(typeof (Entity));
            Assert.AreEqual(0, fields.Count);
        }

        [Test]
        public void Add_duplicate_property_overwrites() {
            var mgr = new MappingManager();
            mgr.Add(typeof (Entity).GetProperty("Id"), "id");
            mgr.Add(typeof (Entity).GetProperty("Id"), "id2");
            var fields = mgr.GetFields(typeof (Entity));
            Assert.AreEqual(1, fields.Count);
            Assert.AreEqual("id2", fields.First().Value.FieldName);
        }

        [Test]
        public void UniqueKey_Set_and_get() {
            var mgr = new MappingManager();
            var property = typeof (Entity).GetProperty("Id");
            mgr.Add(property, "id");
            mgr.SetUniqueKey(property);
            var key = mgr.GetUniqueKey(typeof (Entity));
            Assert.AreEqual(property, key.Property);
            Assert.AreEqual("id", key.FieldName);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void SetUniqueKey_without_mapping_throws() {
            var mgr = new MappingManager();
            var property = typeof (Entity).GetProperty("Id");
            mgr.SetUniqueKey(property);
        }

        [Test]
        public void Add_property_only() {
            var mgr = new MappingManager();
            var property = typeof (Entity).GetProperty("Id");
            mgr.Add(property);
            var fields = mgr.GetFields(typeof (Entity));
            Assert.AreEqual(1, fields.Count);
            Assert.AreEqual("Id", fields.First().Value.FieldName);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void SetUniqueKey_doesnt_admit_null() {
            var mgr = new MappingManager();
            mgr.SetUniqueKey(null);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void GetUniqueKey_doesnt_admit_null() {
            var mgr = new MappingManager();
            mgr.GetUniqueKey(null);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void GetFields_doesnt_admit_null() {
            var mgr = new MappingManager();
            mgr.GetFields(null);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddProperty_doesnt_admit_null() {
            var mgr = new MappingManager();
            mgr.Add(null);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddProperty2_doesnt_admit_null() {
            var mgr = new MappingManager();
            mgr.Add(null, "");
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddProperty3_doesnt_admit_null() {
            var mgr = new MappingManager();
            mgr.Add(typeof (Entity).GetProperties()[0], null);
        }

        [Test]
        [Ignore("Fails, see issue #37")]
        public void Inherited() {
            var mgr = new MappingManager();
            mgr.Add(typeof(Entity).GetProperty("Id"), "id");
            mgr.Add(typeof(InheritedEntity).GetProperty("Description"), "desc");
            var entityFields = mgr.GetFields(typeof (Entity));
            Assert.AreEqual(1, entityFields.Count);
            var inheritedEntityFields = mgr.GetFields(typeof(InheritedEntity));
            Assert.AreEqual(2, inheritedEntityFields.Count);
        }

        [Test]
        public void GetRegistered() {
            var mgr = new MappingManager();
            mgr.Add(typeof(Entity).GetProperty("Id"), "id");
            var types = mgr.GetRegisteredTypes();
            Assert.AreEqual(1, types.Count);
            Assert.Contains(types, typeof(Entity));
        }

    }

    public class Entity {
        public int Id { get; set; }
    }

    public class InheritedEntity: Entity {
        public string Description { get; set; }
    }
}