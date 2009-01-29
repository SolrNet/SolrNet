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
using System.Collections.Generic;
using NUnit.Framework;
using SolrNet.Utils;

namespace SolrNet.Tests {
	[TestFixture]
	public class FuncTests {
		[Test]
		public void Take() {
			var l = new[] {1, 2, 3, 4, 5};
			var r = new List<int>(Func.Take(l, 2));
			Assert.AreEqual(2, r.Count);
			Assert.AreEqual(1, r[0]);
			Assert.AreEqual(2, r[1]);
		}

		[Test]
		public void TakeMore() {
			var l = new[] { 1, 2, 3, 4, 5 };
			var r = new List<int>(Func.Take(l, 200));
			Assert.AreEqual(5, r.Count);
			Assert.AreEqual(1, r[0]);
			Assert.AreEqual(2, r[1]);
		}

		[Test]
		public void First() {
			var l = new[] { 1, 2, 3, 4, 5 };
			Assert.AreEqual(1, Func.First(l));			
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FirstOrDefault_without_elements_throws() {
			var l = new int[] {};
			Func.First(l);
		}
	}
}