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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using SolrNet.Utils;
using IInterceptor = Castle.DynamicProxy.IInterceptor;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace SolrNet.Tests.Utils {
    public class ProfilerFacility : AbstractFacility {
        protected override void Init() {
            Kernel.Register(Component.For<ProfilingInterceptor>().LifeStyle.PerThread);
            Kernel.ComponentModelCreated += model => {
                if (model.Implementation != typeof(ProfilingInterceptor))
                    model.Interceptors.AddFirst(InterceptorReference.ForType<ProfilingInterceptor>());
            };
        }

        public Node<KeyValuePair<MethodInfo, TimeSpan>> GetProfile() {
            return Kernel.Resolve<ProfilingInterceptor>().GetProfile();
        }

        public void Clear() {
            Kernel.Resolve<ProfilingInterceptor>().Clear();
        }

        private class ProfilingInterceptor: IInterceptor {
            private Node<KeyValuePair<MethodInfo, Stopwatch>> currentElement;

            public Node<KeyValuePair<MethodInfo, TimeSpan>> GetProfile() {
                var node = new Node<KeyValuePair<MethodInfo, TimeSpan>>(null, new KeyValuePair<MethodInfo, TimeSpan>(null, new TimeSpan()));
                node.Children.AddRange(currentElement.Children.Select(c => GetProfile(c, node)));
                return node;
            }

            private Node<KeyValuePair<MethodInfo, TimeSpan>> GetProfile(Node<KeyValuePair<MethodInfo, Stopwatch>> n, Node<KeyValuePair<MethodInfo, TimeSpan>> parent) {
                var node = new Node<KeyValuePair<MethodInfo, TimeSpan>>(parent, KV.Create(n.Value.Key, n.Value.Value.Elapsed));
                node.Children.AddRange(n.Children.Select(c => GetProfile(c, node)));
                return node;
            }

            public ProfilingInterceptor() {
                Clear();
            }

            public void Clear() {
                currentElement = new Node<KeyValuePair<MethodInfo, Stopwatch>>(null, new KeyValuePair<MethodInfo, Stopwatch>(null, null));                
            }

            public void Intercept(IInvocation invocation) {
                if (currentElement.Value.Value != null)
                    currentElement.Value.Value.Stop();
                var sw = new Stopwatch();
                var newChild = currentElement.AddChild(KV.Create(invocation.MethodInvocationTarget, sw));
                currentElement = newChild;
                sw.Start();
                try {
                    invocation.Proceed();
                } finally {
                    sw.Stop();
                    currentElement = currentElement.Parent;
                    if (currentElement.Value.Value != null)
                        currentElement.Value.Value.Start();
                }
            }
        }
    }
}