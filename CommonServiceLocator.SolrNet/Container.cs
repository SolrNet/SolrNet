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
using Microsoft.Practices.ServiceLocation;

namespace SolrNet.Utils {
    /// <summary>
    /// Basic built-in dependency-injection container
    /// </summary>
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
            if (!componentsByType.ContainsKey(serviceType))
                yield break;
            foreach (var c in componentsByType[serviceType])
                yield return c(this);
        }

        /// <summary>
        /// Adds a component implementing <typeparamref name="T"/>
        /// Component key is <typeparamref name="T"/>'s <see cref="Type.FullName"/>
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="factory">Component factory method</param>
        public void Register<T>(Converter<IContainer,T> factory) {
            Register(typeof(T).FullName, typeof(T), c => factory(c));
        }

        /// <summary>
        /// Adds a component implementing <typeparamref name="T"/> with the specified key
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="factory">Component factory method</param>
        /// <param name="key">Component key</param>
        public void Register<T>(string key, Converter<IContainer,T> factory) {
            Register(key, typeof(T), c => factory(c));
        }

        /// <summary>
        /// Adds a component
        /// </summary>
        /// <param name="key">Component key</param>
        /// <param name="serviceType">Component service type</param>
        /// <param name="factory">Component factory method. Must return <paramref name="serviceType"/> or a descendant</param>
        public void Register(string key, Type serviceType, Converter<IContainer, object> factory) {
            if (componentsByName.ContainsKey(key))
                throw new ApplicationException(string.Format("Key '{0}' already registered in container", key));
            componentsByName[key] = factory;

            if (!componentsByType.ContainsKey(serviceType))
                componentsByType[serviceType] = new List<Converter<IContainer, object>>();
            componentsByType[serviceType].Add(factory);
        }

        /// <summary>
        /// Removes all components with service type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        public void RemoveAll<T>() {
            if (!componentsByType.ContainsKey(typeof(T)))
                return;
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

        /// <summary>
        /// Removes the default component for service type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        public void Remove<T>() {
            Remove(typeof(T).FullName, typeof(T));
        }

        /// <summary>
        /// Removes the component with key <paramref name="key"/> implementing service type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="key">Component key</param>
        public void Remove<T>(string key) {
            Remove(key, typeof(T));
        }

        /// <summary>
        /// Removes the component with key <paramref name="key"/> implementing service type <paramref name="serviceType"/>
        /// </summary>
        /// <param name="key">Component key</param>
        /// <param name="serviceType">Service type</param>
        public void Remove(string key, Type serviceType) {
            var factory = componentsByName[key];
            componentsByName.Remove(key);
            componentsByType[serviceType].Remove(factory);
        }

        /// <summary>
        /// Removes all component registrations from this container
        /// </summary>
        public void Clear() {
            componentsByType.Clear();
            componentsByName.Clear();
        }
    }
}