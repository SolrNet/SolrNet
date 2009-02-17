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

using System;
using NUnit.Framework;
using SolrNet.Exceptions;
using SolrNet.Mapping;

namespace SolrNet.Tests {
	[TestFixture]
	public class AllPropertiesMappingManagerTests {
		[Test]
		public void GetFields() {
			var m = new AllPropertiesMappingManager();
			var fields = m.GetFields(typeof(Entity));
			Assert.AreEqual(2, fields.Count);
			foreach (var f in fields) {
				if (f.Value == "Id")
					Assert.AreEqual("Id", f.Key.Name);
				else if (f.Value == "Description")
					Assert.AreEqual("Description", f.Key.Name);
				else
					Assert.Fail("Invalid field '{0}'", f.Value);
			}
		}

		[Test]
		public void Get_and_set_unique_key() {
			var m = new AllPropertiesMappingManager();
			m.SetUniqueKey(typeof(Entity).GetProperty("Id"));
			var pk = m.GetUniqueKey(typeof (Entity));
			Assert.IsNotNull(pk);
			Assert.IsNotNull(pk.Key);
			Assert.IsNotNull(pk.Value);
			Assert.AreEqual("Id", pk.Key.Name);
			Assert.AreEqual("Id", pk.Value);
		}

        [Test]
        [ExpectedException(typeof(NoUniqueKeyException))]
        public void NoUniqueKey_ShouldThrow() {
            var m = new AllPropertiesMappingManager();
            var pk = m.GetUniqueKey(typeof(Entity));
        }

        [Test]
        public void NoProperties_ShouldReturnEmpty() {
            var m = new AllPropertiesMappingManager();
            var props = m.GetFields(typeof (NoProperties));
            Assert.AreEqual(0, props.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetUniqueKey_null_throws() {
            var m = new AllPropertiesMappingManager();
            m.SetUniqueKey(null);
        }

        public class NoProperties {}

		public class Entity {
			public int Id { get; set; }

			public string Description { get; set; }
		}
	}
}