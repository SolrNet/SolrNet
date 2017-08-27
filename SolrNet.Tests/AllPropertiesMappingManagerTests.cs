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
using Xunit;
using SolrNet.Exceptions;
using SolrNet.Mapping;

namespace SolrNet.Tests
{

    public class AllPropertiesMappingManagerTests
    {
        [Fact]
        public void GetFields()
        {
            var m = new AllPropertiesMappingManager();
            var fields = m.GetFields(typeof(Entity)).Values;
            Assert.Equal(2, fields.Count);
            foreach (var f in fields)
            {
                if (f.FieldName == "Id")
                    Assert.Equal("Id", f.Property.Name);
                else if (f.FieldName == "Description")
                    Assert.Equal("Description", f.Property.Name);
                else
                    Assert.True(false,String.Format("Invalid field '{0}'", f.FieldName));
            }
        }

        [Fact]
        public void Get_and_set_unique_key()
        {
            var m = new AllPropertiesMappingManager();
            m.SetUniqueKey(typeof(Entity).GetProperty("Id"));
            var pk = m.GetUniqueKey(typeof(Entity));
            Assert.NotNull(pk);
            Assert.NotNull(pk.Property);
            Assert.NotNull(pk.FieldName);
            Assert.Equal("Id", pk.Property.Name);
            Assert.Equal("Id", pk.FieldName);
        }

        [Fact]
        public void NoUniqueKey_IsNull()
        {
            var m = new AllPropertiesMappingManager();
            var pk = m.GetUniqueKey(typeof(Entity));
            Assert.Null(pk);
        }

        [Fact]
        public void NoProperties_ShouldReturnEmpty()
        {
            var m = new AllPropertiesMappingManager();
            var props = m.GetFields(typeof(NoProperties));
            Assert.Equal(0, props.Count);
        }

        [Fact]
        public void SetUniqueKey_null_throws()
        {
            var m = new AllPropertiesMappingManager();
            Assert.Throws<ArgumentNullException>(() => m.SetUniqueKey(null));
        }

        [Fact]
        public void Inherited()
        {
            var m = new AllPropertiesMappingManager();
            var fields = m.GetFields(typeof(InheritedEntity));
            Assert.Equal(3, fields.Count);
        }

        [Fact]
        public void GetRegistered()
        {
            var m = new AllPropertiesMappingManager();
            var types = m.GetRegisteredTypes();
            Assert.Empty(types);
        }

        public class NoProperties { }

        public class Entity
        {
            public int Id { get; set; }

            public string Description { get; set; }
        }

        public class InheritedEntity : Entity
        {
            public DateTime DateOfBirth { get; set; }
        }
    }
}