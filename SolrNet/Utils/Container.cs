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
using Microsoft.Practices.ServiceLocation;

namespace SolrNet.Utils {
    /// <summary>
    /// Basic built-in dependency-injection container.
    /// </summary>
    public class Container : ServiceLocatorImplBase, IContainer {
        private readonly Dictionary<string, IEnumerable<Converter<IContainer, object>>> componentCollections = new Dictionary<string, IEnumerable<Converter<IContainer, object>>>();
        private readonly Dictionary<string, Converter<IContainer, object>> components = new Dictionary<string, Converter<IContainer, object>>();
        private readonly Dictionary<string, IList<string>> typeRegistry = new Dictionary<string, IList<string>>();
        private readonly Dictionary<string, bool> keyRegistry = new Dictionary<string, bool>();

        public Container() {}

        /// <summary>
        /// Creates a new container copying all components from another container.
        /// </summary>
        /// <param name="c"></param>
        public Container(Container c) {
            componentCollections = new Dictionary<string, IEnumerable<Converter<IContainer, object>>>(c.componentCollections);
            components = new Dictionary<string, Converter<IContainer, object>>(c.components);
            typeRegistry = new Dictionary<string, IList<string>>(c.typeRegistry);
            keyRegistry = new Dictionary<string, bool>(c.keyRegistry);
        }

        #region ServiceLocator callbacks

        protected override object DoGetInstance(Type serviceType, string key) {
            var componentKey = BuildComponentKey(key, serviceType);

            return components[componentKey](this);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            var componentKey = BuildComponentKey(null, serviceType);

            if (!componentCollections.ContainsKey(componentKey)) yield break;
            foreach (var component in componentCollections[componentKey]) {
                yield return component(this);
            }
        }

        #endregion ServiceLocator callbacks

        #region Component management

        /// <summary>
        /// Adds a collection of components for service type <typeparamref name="T"/>.
        /// Component key is <typeparamref name="T"/>'s <see cref="Type.FullName"/>.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="factories">Component factory method collection.</param>
        public void RegisterAll<T>(IEnumerable<Converter<IContainer, T>> factories) {
            var componentKey = RegisterComponentKey(null, typeof(T));
            componentCollections.Add(componentKey, Func.Cast<Converter<IContainer, object>>(factories));
        }

        /// <summary>
        /// Adds a default component for service type <typeparamref name="T"/>.
        /// Component key is <typeparamref name="T"/>'s <see cref="Type.FullName"/>.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="factory">Component factory method.</param>
        public void Register<T>(Converter<IContainer,T> factory) {
            Register(null, typeof(T), c => factory(c));
        }

        /// <summary>
        /// Adds a component with key <paramref name="key"/> for service type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="factory">Component factory method.</param>
        /// <param name="key">Component key.</param>
        public void Register<T>(string key, Converter<IContainer,T> factory) {
            Register(key, typeof(T), c => factory(c));
        }

        /// <summary>
        /// Adds a component.
        /// </summary>
        /// <param name="key">Component key.</param>
        /// <param name="serviceType">Component service type.</param>
        /// <param name="factory">Component factory method. Must return <paramref name="serviceType"/> or a descendant</param>
        public void Register(string key, Type serviceType, Converter<IContainer, object> factory) {
            var componentKey = RegisterComponentKey(key, serviceType);
            components[componentKey] = factory;
        }

        /// <summary>
        /// Removes all collections of components for service type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        public void RemoveAll<T>() {
            var typeKey = BuildTypeKey(typeof(T));
            var deregisterList = new List<string>();

            foreach (var key in typeRegistry[typeKey]) {
                var componentKey = BuildComponentKey(key, typeof(T));
                componentCollections.Remove(componentKey);
                components.Remove(componentKey);
                deregisterList.Add(key);
            }
            deregisterList.ForEach(key => DeregisterComponentKey(key, typeof(T)));
        }

        /// <summary>
        /// Removes the default component for service type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        public void Remove<T>() {
            Remove(null, typeof(T));
        }

        /// <summary>
        /// Removes the component with key <paramref name="key"/> for service type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="key">Component key</param>
        public void Remove<T>(string key) {
            Remove(key, typeof(T));
        }

        /// <summary>
        /// Removes the component with key <paramref name="key"/> for service type <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="key">Component key</param>
        /// <param name="serviceType">Service type</param>
        public void Remove(string key, Type serviceType) {
            var componentKey = BuildComponentKey(key, serviceType);
            components.Remove(componentKey);
            DeregisterComponentKey(key, serviceType);
        }

        /// <summary>
        /// Replaces a collection of components for service type <typeparamref name="T"/>.
        /// Component key is <typeparamref name="T"/>'s <see cref="Type.FullName"/>.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="factories">Component factory method collection.</param>
        public void ReplaceAll<T>(IEnumerable<Converter<IContainer, T>> factories)
        {
            var typeKey = BuildTypeKey(typeof(T));
            var deregisterList = new List<string>();

            foreach (var key in typeRegistry[typeKey])
            {
                var componentKey = BuildComponentKey(key, typeof(T));
                componentCollections.Remove(componentKey);
                deregisterList.Add(key);
            }
            deregisterList.ForEach(key => DeregisterComponentKey(key, typeof(T)));

            RegisterAll<T>(factories);
        }

        /// <summary>
        /// Replaces a default component for service type <typeparamref name="T"/>.
        /// Component key is <typeparamref name="T"/>'s <see cref="Type.FullName"/>.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="factory">Component factory method.</param>
        public void Replace<T>(Converter<IContainer, T> factory)
        {
            Remove<T>();
            Register<T>(factory);
        }

        /// <summary>
        /// Replaces a component with key <paramref name="key"/> for service type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="factory">Component factory method.</param>
        /// <param name="key">Component key.</param>
        public void Replace<T>(string key, Converter<IContainer, T> factory)
        {
          Remove<T>(key);
          Register<T>(key, factory);
        }

        /// <summary>
        /// Replaces a component.
        /// </summary>
        /// <param name="key">Component key.</param>
        /// <param name="serviceType">Component service type.</param>
        /// <param name="factory">Component factory method. Must return <paramref name="serviceType"/> or a descendant</param>
        public void Replace(string key, Type serviceType, Converter<IContainer, object> factory)
        {
          Remove(key, serviceType);
          Register(key, serviceType, factory);
        }

        /// <summary>
        /// Removes all component registrations from this container.
        /// </summary>
        public void Clear() {
            componentCollections.Clear();
            components.Clear();
            keyRegistry.Clear();
            typeRegistry.Clear();
        }

        #endregion Component management

        #region Helpers

        /// <summary>
        /// Builds the component key from the provided key <paramref name="key"/> and type <paramref name="type"/>.
        /// </summary>
        /// <param name="key">Component key.</param>
        /// <param name="type">Component type.</param>
        /// <returns>The key to access components within the container.</returns>
        private string BuildComponentKey(string key, Type type) {
            return (string.IsNullOrEmpty(key))
                ? type.FullName
                : key + "|" + type.FullName;
        }

        /// <summary>
        /// Builds the type key from the provided type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Component type.</param>
        /// <returns>The type key for the type registry.</returns>
        private string BuildTypeKey(Type type) {
          return type.FullName;
        }

        /// <summary>
        /// Builds the component key from the provided key <paramref name="key"/> and type <paramref name="type"/>. Then
        /// registers the key and type in their respective registries. Checks that this key has not already been added.
        /// </summary>
        /// <param name="key">Component key.</param>
        /// <param name="type">Component type.</param>
        /// <returns>The key to access components within the container.</returns>
        private string RegisterComponentKey(string key, Type type) {
            var typeKey = BuildTypeKey(type);
            var componentKey = BuildComponentKey(key, type);

            if (keyRegistry.ContainsKey(componentKey)) {
                throw new ApplicationException(string.Format("Key '{0}' already registered in container.", componentKey));
            }
            keyRegistry[componentKey] = true;

            if (!typeRegistry.ContainsKey(typeKey)) {
                typeRegistry[typeKey] = new List<string>();
            }
            typeRegistry[typeKey].Add(key);

            return componentKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        private void DeregisterComponentKey(string key, Type type) {
            var typeKey = BuildTypeKey(type);
            var componentKey = BuildComponentKey(key, type);

            keyRegistry.Remove(componentKey);

            if (!typeRegistry.ContainsKey(typeKey)) return;
            typeRegistry[typeKey].Remove(key);

            if (typeRegistry[typeKey].Count == 0) {
                typeRegistry.Remove(typeKey);
            }
        }

        #endregion Helpers
    }
}