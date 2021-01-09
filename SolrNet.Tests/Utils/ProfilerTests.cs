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
using System.Reflection;
using System.Threading;
using Castle.MicroKernel.Registration;
using Xunit;
using Xunit.Abstractions;

namespace SolrNet.Tests.Utils {
    
    public class ProfilerTests {
        private readonly ITestOutputHelper testOutputHelper;

        public ProfilerTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

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

        [Fact]
        public void NonProxyableComponent() {
            var container = new ProfilingContainer();
            container.Register(Component.For<NonProxyable>());
            container.Resolve<NonProxyable>().LongOperation();
            var profile = container.GetProfile();
            Assert.Empty(profile.Children);
        }

        // Y-combinator
        public delegate Func<A, R> Recursive<A, R>(Recursive<A, R> r);
        public static Func<A, R> Y<A, R>(Func<Func<A, R>, Func<A, R>> f) {
            Recursive<A, R> rec = r => a => f(r(r))(a); 
            return rec(rec);
        }

        public IEnumerable<KeyValuePair<MethodInfo, TimeSpan>> Flatten(Node<KeyValuePair<MethodInfo, TimeSpan>> n) {
            if (n.Value.Key != null)
                yield return n.Value;
            foreach (var i in n.Children.SelectMany(c => Flatten(c)))
                yield return i;
        }

        [Fact]
        public void ProxyableComponent() {
            var container = new ProfilingContainer();
            container.Register(Component.For<Proxyable>());
            container.Resolve<Proxyable>().LongOperation();
            var profile = container.GetProfile();
            Assert.Single(profile.Children);
            testOutputHelper.WriteLine("{0}: {1}", profile.Children[0].Value.Key, profile.Children[0].Value.Value);
            var fProfile = Flatten(profile);
            var q = from n in fProfile
                    group n.Value by n.Key into x
                    let kv = new KeyValuePair<MethodInfo, double>(x.Key, x.Sum(t => t.TotalMilliseconds))
                    orderby kv.Value descending
                    select kv;

            foreach (var i in q)
                testOutputHelper.WriteLine("{0}: {1}", i.Key, i.Value);

            //Console.WriteLine(profile.Values.ToList()[0][0].TotalMilliseconds);
        }
    }
}
