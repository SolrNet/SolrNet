using System;
using Microsoft.Practices.ServiceLocation;

namespace SolrNet.Utils {
    public interface IContainer : IServiceLocator {
        /// <summary>
        /// Adds a component implementing <typeparamref name="T"/>
        /// Component key is <typeparamref name="T"/>'s <see cref="Type.FullName"/>
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="factory">Component factory method</param>
        void Register<T>(Converter<IContainer,T> factory);

        /// <summary>
        /// Adds a component implementing <typeparamref name="T"/> with the specified key
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="factory">Component factory method</param>
        /// <param name="key">Component key</param>
        void Register<T>(string key, Converter<IContainer,T> factory);

        /// <summary>
        /// Adds a component
        /// </summary>
        /// <param name="key">Component key</param>
        /// <param name="serviceType">Component service type</param>
        /// <param name="factory">Component factory method. Must return <paramref name="serviceType"/> or a descendant</param>
        void Register(string key, Type serviceType, Converter<IContainer, object> factory);

        /// <summary>
        /// Removes all components with service type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        void RemoveAll<T>();

        /// <summary>
        /// Removes the default component for service type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        void Remove<T>();

        /// <summary>
        /// Removes the component with key <paramref name="key"/> implementing service type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="key">Component key</param>
        void Remove<T>(string key);

        /// <summary>
        /// Removes the component with key <paramref name="key"/> implementing service type <paramref name="serviceType"/>
        /// </summary>
        /// <param name="key">Component key</param>
        /// <param name="serviceType">Service type</param>
        void Remove(string key, Type serviceType);
    }
}