using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace SolrNet.Utils {
    public class Container : ServiceLocatorImplBase, IContainer {
        private readonly Dictionary<string, Converter<IContainer, object>> componentsByName = new Dictionary<string, Converter<IContainer, object>>();
        private readonly Dictionary<Type, List<Converter<IContainer, object>>> componentsByType = new Dictionary<Type, List<Converter<IContainer, object>>>();

        public Container() {}

        /// <summary>
        /// Creates a new container copying all components from another container
        /// </summary>
        /// <param name="c"></param>
        public Container(Container c) {
            componentsByName = new Dictionary<string, Converter<IContainer, object>>(c.componentsByName);
            foreach (var t in c.componentsByType)
                componentsByType[t.Key] = new List<Converter<IContainer, object>>(t.Value);
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            if (key == null)
                return componentsByType[serviceType][0](this);
            return componentsByName[key](this);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            foreach (var c in componentsByType[serviceType])
                yield return c(this);
        }

        public void Register<T>(Converter<IContainer,T> factory) {
            Register(typeof(T).FullName, typeof(T), c => factory(c));
        }

        public void Register<T>(string key, Converter<IContainer,T> factory) {
            Register(key, typeof(T), c => factory(c));
        }

        public void Register(string key, Type serviceType, Converter<IContainer, object> factory) {
            if (componentsByName.ContainsKey(key))
                throw new ApplicationException(string.Format("Key '{0}' already registered in container", key));
            componentsByName[key] = factory;

            if (!componentsByType.ContainsKey(serviceType))
                componentsByType[serviceType] = new List<Converter<IContainer, object>>();
            componentsByType[serviceType].Add(factory);
        }

        public void RemoveAll<T>() {
            foreach (var c in componentsByType[typeof(T)]) {
                var removeList = new List<string>();
                foreach (var cn in componentsByName) {
                    if (cn.Value == c)
                        removeList.Add(cn.Key);
                }
                removeList.ForEach(k => componentsByName.Remove(k));
            }
            componentsByType[typeof(T)].Clear();
        }

        public void Remove<T>() {
            Remove(typeof(T).FullName, typeof(T));
        }

        public void Remove<T>(string key) {
            Remove(key, typeof(T));
        }

        public void Remove(string key, Type serviceType) {
            var factory = componentsByName[key];
            componentsByName.Remove(key);
            componentsByType[serviceType].Remove(factory);
        }
    }
}