using System;
using Moroco;

namespace SolrNet.Tests.Mocks {
    public class MServiceProvider : IServiceProvider {
        public MFunc<Type, object> getService;

        public object GetService(Type serviceType) {
            return getService.Invoke(serviceType);
        }
    }
}