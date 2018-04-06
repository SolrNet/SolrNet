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
using Xunit;
using SolrNet.Exceptions;
using SolrNet.Mapping;

namespace SolrNet.Tests {
    
    public class MappingManagerTests {
        [Fact]
        public void AddAndGet() {
            var mgr = new MappingManager();
            mgr.Add(typeof (Entity).GetProperty("Id"), "id");
            var fields = mgr.GetFields(typeof (Entity));
            Assert.Equal(1, fields.Count);
        }

        [Fact]
        public void No_Mapped_type_returns_empty() {
            var mgr = new MappingManager();
            var fields = mgr.GetFields(typeof (Entity));
            Assert.Equal(0, fields.Count);
        }

        [Fact]
        public void Add_duplicate_property_overwrites() {
            var mgr = new MappingManager();
            mgr.Add(typeof (Entity).GetProperty("Id"), "id");
            mgr.Add(typeof (Entity).GetProperty("Id"), "id2");
            var fields = mgr.GetFields(typeof (Entity));
            Assert.Equal(1, fields.Count);
            Assert.Equal("id2", fields.First().Value.FieldName);
        }

        [Fact]
        public void UniqueKey_Set_and_get() {
            var mgr = new MappingManager();
            var property = typeof (Entity).GetProperty("Id");
            mgr.Add(property, "id");
            mgr.SetUniqueKey(property);
            var key = mgr.GetUniqueKey(typeof (Entity));

            Assert.Equal(property, key.Property);
            Assert.Equal("id", key.FieldName);
        }

        [Fact]
        public void UniqueKey_Set_and_get_for_inherited_classes()
        {
            var mgr = new MappingManager();
            var property = typeof(Entity).GetProperty("Id");
            mgr.Add(property, "id");
            mgr.SetUniqueKey(property);
            var key = mgr.GetUniqueKey(typeof(InheritedEntity));

            Assert.Equal(property, key.Property);
            Assert.Equal("id", key.FieldName);
        }

        [Fact]
        public void InheritedEntityMappedUsingReflectedTypeOnly()
        {
            var mappingManager = new MappingManager(true);
            var idProperty = typeof(ComplexEntity<FrenchSchemaLanguage>).GetProperty("Id");
            mappingManager.Add(idProperty, "id");
            mappingManager.SetUniqueKey(idProperty);
            mappingManager.Add(typeof(ComplexEntity<FrenchSchemaLanguage>).GetProperty("Description"), "description");

            var registeredType = mappingManager.GetRegisteredTypes();
            Assert.Equal(1, registeredType.Count);
            Assert.Single(registeredType, typeof(ComplexEntity<FrenchSchemaLanguage>));
        }

        [Fact]
        public void InheritedEntityMappedNotUsingReflectedTypeOnly()
        {
            var mappingManager = new MappingManager(false); // Same as "new MappingManager();"
            var idProperty = typeof(ComplexEntity<FrenchSchemaLanguage>).GetProperty("Id");
            mappingManager.Add(idProperty, "id");
            mappingManager.SetUniqueKey(idProperty);
            mappingManager.Add(typeof(ComplexEntity<FrenchSchemaLanguage>).GetProperty("Description"), "description");

            var registeredType = mappingManager.GetRegisteredTypes();
            Assert.Equal(2, registeredType.Count);
            Assert.Collection(registeredType, type => Assert.Equal(typeof(BaseComplexEntity<FrenchSchemaLanguage>), type),
                type => Assert.Equal(typeof(ComplexEntity<FrenchSchemaLanguage>), type));
        }

        [Fact]
        public void SetUniqueKey_without_mapping_throws() {
            var mgr = new MappingManager();
            var property = typeof (Entity).GetProperty("Id");
            Assert.Throws<ArgumentException>(() => mgr.SetUniqueKey(property));
        }

        [Fact]
        public void Add_property_only() {
            var mgr = new MappingManager();
            var property = typeof (Entity).GetProperty("Id");
            mgr.Add(property);
            var fields = mgr.GetFields(typeof (Entity));
            Assert.Equal(1, fields.Count);
            Assert.Equal("Id", fields.First().Value.FieldName);
        }

        [Fact]
        public void SetUniqueKey_doesnt_admit_null() {
            var mgr = new MappingManager();
            Assert.Throws<ArgumentNullException>(() => mgr.SetUniqueKey(null));
        }

        [Fact]
        public void GetUniqueKey_doesnt_admit_null() {
            var mgr = new MappingManager();
            Assert.Throws<ArgumentNullException>(() => mgr.GetUniqueKey(null));
        }

        [Fact]
        
        public void GetFields_doesnt_admit_null() {
            var mgr = new MappingManager();
            Assert.Throws<ArgumentNullException>(() => mgr.GetFields(null));
        }

        [Fact]
        public void AddProperty_doesnt_admit_null() {
            var mgr = new MappingManager();
            Assert.Throws<ArgumentNullException>(() => mgr.Add(null));
        }

        [Fact]
        public void AddProperty2_doesnt_admit_null() {
            var mgr = new MappingManager();
            Assert.Throws<ArgumentNullException>(() => mgr.Add(null, ""));
        }

        [Fact]
        public void AddProperty3_doesnt_admit_null() {
            var mgr = new MappingManager();
            Assert.Throws<ArgumentNullException>(() => mgr.Add(typeof (Entity).GetProperties()[0], null));
        }

        [Fact]
        public void Inherited() {
            var mgr = new MappingManager();
            mgr.Add(typeof(Entity).GetProperty("Id"), "id");
            mgr.Add(typeof(InheritedEntity).GetProperty("Description"), "desc");
            var entityFields = mgr.GetFields(typeof(Entity));
            Assert.Equal(1, entityFields.Count);
            var inheritedEntityFields = mgr.GetFields(typeof(InheritedEntity));
            Assert.Equal(2, inheritedEntityFields.Count);
        }

		[Fact]
		public void Inherited_gets_id_property_correctly()
		{
			var mgr = new MappingManager();
			mgr.Add(typeof(Entity).GetProperty("Id"), "id");

			Assert.True(mgr.GetFields(typeof(Entity)).ContainsKey("id"), "Entity contains id field");
			Assert.True(mgr.GetFields(typeof(InheritedEntity)).ContainsKey("id"), "InheritedEntity contains id field");
		}

		[Fact]
		public void Inherited_gets_id_property_correctly2()
		{
			var mgr = new MappingManager();
			mgr.Add(typeof(InheritedEntity).GetProperty("Id"), "id");

			Assert.True(mgr.GetFields(typeof(InheritedEntity)).ContainsKey("id"), "InheritedEntity contains id field");
			Assert.True(mgr.GetFields(typeof(Entity)).ContainsKey("id"), "Entity contains id field");
		}

        [Fact]
        public void GetRegistered() {
            var mgr = new MappingManager();
            mgr.Add(typeof(Entity).GetProperty("Id"), "id");
            var types = mgr.GetRegisteredTypes();
            Assert.Equal(1, types.Count);
            Assert.Contains( typeof(Entity),types);
        }

    }

    public class Entity {
        public int Id { get; set; }
    }

    public class InheritedEntity: Entity {
        public string Description { get; set; }
    }

    public class ComplexEntity<T> : BaseComplexEntity<T>
        where T : SchemaLanguage, new()
    {
        public string Description { get; set; }
    }

    public class BaseComplexEntity<T> where T : SchemaLanguage, new()
    {
        public int Id { get; set; }
        public T Language => new T();
    }

    public abstract class SchemaLanguage
    {
        public string LanguageCode { get; set; }
    }

    public class FrenchSchemaLanguage : SchemaLanguage
    {
        public FrenchSchemaLanguage()
        {
            LanguageCode = "fr";
        }
    }

    public class EnglishSchemaLanguage : SchemaLanguage
    {
        public EnglishSchemaLanguage()
        {
            LanguageCode = "en";
        }
    }
}
