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
using System.Threading;
using MbUnit.Framework;

namespace SolrNet.Tests.Utils {
    [TestFixture]
    public class ProfilerTests {
        public class NonProxyable {
            public void LongOperation() {
                Thread.Sleep(1000);
            }
        }

        public class Proxyable {
            public virtual void LongOperation() {
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void NonProxyableComponent() {
            var container = new ProfilingContainer();
            container.AddComponent<NonProxyable>();
            container.Resolve<NonProxyable>().LongOperation();
            var profile = container.GetProfile();
            Assert.AreEqual(0, profile.Count);
        }

        [Test]
        public void ProxyableComponent() {
            var container = new ProfilingContainer();
            container.AddComponent<Proxyable>();
            container.Resolve<Proxyable>().LongOperation();
            var profile = container.GetProfile();
            Assert.AreEqual(1, profile.Count);
            Console.WriteLine(profile.Values.ToList()[0][0].TotalMilliseconds);
        }
    }
}