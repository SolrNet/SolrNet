using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Moroco;

namespace SolrNet.Tests.Mocks {
    public class MServiceLocator : IServiceLocator {
        public MFunc<Type, object> getService;

        public static MFunc<Type, object> Table(IDictionary<Type, object> services) {
            return new MFunc<Type, object>(t => services[t]);
        }

        public static MFunc<Type, object> One<I>(object o) {
            return new MFunc<Type, object>(t => {
                if (t == typeof (I))
                    return o;
                throw new ArgumentException("Unexpected type " + t);
            });
        }

        public object GetService(Type serviceType) {
            return getService.Invoke(serviceType);
        }

        public object GetInstance(Type serviceType) {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, string key) {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType) {
            throw new NotImplementedException();
        }

        public Func<Type, object> getInstance;

        public TService GetInstance<TService>() {
            return (TService) getInstance.Invoke(typeof(TService));
        }

        public TService GetInstance<TService>(string key) {
            throw new NotImplementedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>() {
            throw new NotImplementedException();
        }
    }
}