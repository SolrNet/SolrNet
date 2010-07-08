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
using System.Collections;
using System.IO;
using MbUnit.Framework;
using SolrNet.Impl;

namespace SolrNet.Tests {
    [TestFixture]
    public class JavaBinCodecTests {
        [Test]
        [Row("hello world")]
        [Row(55)] // integer
        [Row(56L)] // long
        [Row((short)57)] // short
        [Row(123.45)] // double
        [Row(123.45f)] // float
        [Row(true)] // bool true
        [Row(false)] // bool false
        [Row(null)] // null
        [Row((byte)200)] // byte
        [Factory("DateTime")]
        public void MarshalUnmarshal(object obj) {
            using (var ms = new MemoryStream()) {
                new JavaBinCodec().Marshal(obj, ms);
                ms.Position = 0;
                var s = new JavaBinCodec().Unmarshal(ms);
                if (obj == null)
                    Assert.IsNull(s);
                else
                    Assert.IsInstanceOfType(obj.GetType(), s);
                Assert.AreEqual(obj, s);
            }
        }

        public IEnumerable DateTime() {
            yield return new DateTime(2009, 10, 1, 6, 7, 8);
        }
    }
}