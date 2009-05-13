using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Proxy;

namespace SolrNet.Tests {
    public class ProfilerFacility : AbstractFacility {
        protected override void Init() {
            Kernel.AddComponent<ProfilingInterceptor>();
            Kernel.ProxyFactory.AddInterceptorSelector(new ModelInterceptorSelector());
        }

        public Dictionary<MethodInfo, List<TimeSpan>> GetProfile() {
            return Kernel.Resolve<ProfilingInterceptor>().GetProfile();
        }

        private class ModelInterceptorSelector : IModelInterceptorsSelector {
            public InterceptorReference[] SelectInterceptors(ComponentModel model) {
                return new[] {InterceptorReference.ForType<ProfilingInterceptor>()};
            }

            public bool HasInterceptors(ComponentModel model) {
                return model.Implementation != typeof (ProfilingInterceptor);
            }
        }


        private class ProfilingInterceptor : IInterceptor {
            private readonly Dictionary<MethodInfo, List<Stopwatch>> methods = new Dictionary<MethodInfo, List<Stopwatch>>();
            private readonly Stack<MethodInfo> methodStack = new Stack<MethodInfo>();

            private static KeyValuePair<K, V> KV<K, V>(K key, V value) {
                return new KeyValuePair<K, V>(key, value);
            }

            public Dictionary<MethodInfo, List<TimeSpan>> GetProfile() {
                return methods.Select(k => KV(k.Key, k.Value.Select(s => s.Elapsed).ToList()))
                    .ToDictionary(k => k.Key, k => k.Value);
            }

            private Stopwatch NewStopwatch(MethodInfo m) {
                if (!methods.ContainsKey(m))
                    methods[m] = new List<Stopwatch>();
                var r = new Stopwatch();
                methods[m].Add(r);
                return r;
            }

            private MethodInfo GetPreviousMethod() {
                if (methodStack.Count == 0)
                    return null;
                return methodStack.Peek();
            }

            public void Intercept(IInvocation invocation) {
                var prevMethod = GetPreviousMethod();
                if (prevMethod != null) {
                    var l = methods[prevMethod];
                    l[l.Count - 1].Stop();
                }
                methodStack.Push(invocation.MethodInvocationTarget);
                var sw = NewStopwatch(invocation.MethodInvocationTarget);
                try {
                    sw.Start();
                    invocation.Proceed();
                } finally {
                    sw.Stop();
                    methodStack.Pop();
                    if (prevMethod != null) {
                        var l = methods[prevMethod];
                        l[l.Count - 1].Start();
                    }
                }
            }
        }
    }
}