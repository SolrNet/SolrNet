using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Windsor;

namespace SolrNet.Tests {
    public class ProfilingContainer : WindsorContainer {
        private readonly ProfilerFacility profiler;

        public ProfilingContainer() {
            profiler = new ProfilerFacility();
            AddFacility("profiler", profiler);
        }

        public Dictionary<MethodInfo, List<TimeSpan>> GetProfile() {
            return profiler.GetProfile();
        }
    }
}