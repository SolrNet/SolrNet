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
using SolrNet.Attributes;
using SolrNet.Exceptions;
using SolrNet.Mapping;

namespace SolrNet.Tests {
	
	public class AttributesMappingManagerTests {
		[Fact]
		public void GetFields() {
			var m = new AttributesMappingManager();
			var fields = m.GetFields(typeof (Entity)).Values;
			Assert.Equal(2, fields.Count);
			foreach (var f in fields) {
				if (f.FieldName == "Id")
					Assert.Equal("Id", f.Property.Name);
				else if (f.FieldName == "desc")
					Assert.Equal("Description", f.Property.Name);
				else
					Assert.True(false, string.Format("Invalid field '{0}'", f.FieldName));
			}
		}

        [Fact]
        public void GetFields_WithDuplicatesProperties()
        {
            var m = new AttributesMappingManager();
            Assert.Throws<SolrNetException>(() => m.GetFields(typeof(EntityWithDuplicateFields)).Values);
          
        }

        [Fact]
		public void GetUniqueKey() {
			var m = new AttributesMappingManager();
			var key = m.GetUniqueKey(typeof (Entity));
			Assert.NotNull(key);
			Assert.NotNull(key.Property);
			Assert.Equal("Id", key.Property.Name);
			Assert.Equal("Id", key.FieldName);
		}

        [Fact]
        public void DifferentTypes() {
            var m = new AttributesMappingManager();
            var key = m.GetUniqueKey(typeof(Entity));
            Assert.NotNull(key);
            Assert.NotNull(key.Property);
            Assert.Equal("Id", key.Property.Name);
            Assert.Equal("Id", key.FieldName);
            var fields = m.GetFields(typeof (AnotherEntity));
            Assert.Equal(1, fields.Count);
        }

        [Fact]
        public void NoProperties_ShouldReturnEmpty() {
            var m = new AttributesMappingManager();
            var fields = m.GetFields(typeof (NoProperties));
            Assert.Equal(0, fields.Count);
        }

        [Fact]
        public void GetUniqueKey_without_unique_key_throws() {
            var m = new AttributesMappingManager();
            var pk = m.GetUniqueKey(typeof (AnotherEntity));
            Assert.Null(pk);
        }

        [Fact]
        public void Inherited() {
            var m = new AttributesMappingManager();
            var fields = m.GetFields(typeof (InheritedEntity));
            Assert.Equal(3, fields.Count);
            var uniqueKey = m.GetUniqueKey(typeof(InheritedEntity));
            Assert.NotNull(uniqueKey);
            Assert.Equal("Id", uniqueKey.FieldName);
        }

        [Fact]
        public void GetRegisteredTypes() {
            var m = new AttributesMappingManager();
            var types = m.GetRegisteredTypes();

            Assert.True(types.Count > 0);
            Assert.Contains( typeof(Entity), types);
            Assert.Contains( typeof(InheritedEntity), types);
            Assert.Contains(typeof(AnotherEntity), types);
            Assert.DoesNotContain( typeof(NoProperties), types);
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

        public class EntityWithDuplicateFields
        {
            [SolrField("foo")]
            public string Prop1 { get; set; }

            [SolrField("foo")]
            public string Prop2 { get; set; }
        }
	}
}
