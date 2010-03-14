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
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
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
		public void First_without_elements_throws() {
			var l = new int[] {};
			Func.First(l);
		}

        [Test]
        public void FirstOrDefault_without_elements_default() {
            var l = new int[] { };
            Assert.AreEqual(default(int), Func.FirstOrDefault(l));
        }

        [Test]
        public void ConvertStringToInt() {
            var l = new[] {"1", "2", "358"};
            Assert.AreElementsEqual(new[] {1, 2, 358}, Func.Convert<int>(l));
        }

        [Test]
        public void ConvertStringToInt_invalid() {
            var l = new[] { "1", "2", "pepe" };
            Assert.Throws<FormatException>(() => Func.Convert<int>(l).ToList());
        }

        [Test]
        public void Any_true() {
            Assert.IsTrue(Func.Any(new[] { 1, 2, 3 }, i => i > 2));
        }

        [Test]
        public void Any_false() {
            Assert.IsFalse(Func.Any(new[] { 1, 2, 3 }, i => i > 3));
        }

        [Test]
        public void ToArray() {
            int[] l = Func.ToArray(Enumerable.Range(0, 5));
            Assert.AreElementsEqual(new[] {0,1,2,3,4}, l);
        }

        [Test]
        public void Distinct() {
            var l = new[] {1, 1, 2, 3, 2, 4, 5};
            Assert.AreElementsEqual(new[] {1,2,3,4,5}, Func.Distinct(l));
        }

        [Test]
        public void Skip() {
            var l = new[] { 1, 2, 3, 4, 5 };
            Assert.AreElementsEqual(new[] {2,3,4,5}, Func.Skip(l, 1));
        }
	}
}