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
using System.Linq;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrFacetFieldQueryTests {
		[Test]
		public void FieldOnly() {
			var fq = new SolrFacetFieldQuery("pepe");
			var q = fq.Query.ToList();
			Assert.AreEqual(1, q.Count);
			Console.WriteLine(q[0]);
			Assert.AreEqual("facet.field", q[0].Key);
			Assert.AreEqual("pepe", q[0].Value);
		}
	}
}