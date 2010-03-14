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

using System.Collections;
using System.Collections.Generic;
using MbUnit.Framework;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class TypeHelperTests {
        [Test]
        public void IsGenericAssignableFrom() {
            Assert.IsTrue(TypeHelper.IsGenericAssignableFrom(typeof (IDictionary<,>), typeof (Dictionary<string, string>)));
        }

        [Test]
        public void IsNotGenericAssignableFrom() {
            Assert.IsFalse(TypeHelper.IsGenericAssignableFrom(typeof(IDictionary<,>), typeof(List<string>)));
        }

        [Test]
        public void IsNotGenericAssignableFrom_non_generic() {
            Assert.IsFalse(TypeHelper.IsGenericAssignableFrom(typeof(IList), typeof(List<string>)));
        }

        [Test]
        public void IsNullableType() {
            Assert.IsTrue(TypeHelper.IsNullableType(typeof (int?)));
        }

        [Test]
        public void IsNotNullableType() {
            Assert.IsFalse(TypeHelper.IsNullableType(typeof(int)));
        }

    }
}