﻿using System;
using System.Collections.Generic;
using SolrNet.Utils;
using Parent = SolrNet.Startup;
using System.Threading.Tasks;
using HttpWebAdapters;

namespace SolrNet.Cloud
{
    /// <summary>
    /// Startup helper for cloud mode
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Collection list
        /// </summary>
        private static readonly HashSet<string> Collections;

        /// <summary>
        /// Cloud state providers
        /// </summary>
        private static readonly IDictionary<string, ISolrCloudStateProvider> Providers;

        /// <summary>
        /// Container instance
        /// </summary>
        public static IContainer Container
        {
            get { return Parent.Container; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        static Startup()
        {
            Collections = new HashSet<string>();
            Providers = new Dictionary<string, ISolrCloudStateProvider>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Startup initializing 
        /// </summary>
        public static async Task InitAsync<T>(ISolrCloudStateProvider cloudStateProvider, bool isPostConnection = false)
        {
            if (cloudStateProvider == null)
                throw new ArgumentNullException("cloudStateProvider");
            await EnsureRegistrationAsync(cloudStateProvider);

            if (!Collections.Add(string.Empty))
                return;

            Parent.Container.Register<ISolrBasicOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    isPostConnection));

            Parent.Container.Register<ISolrBasicReadOnlyOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    isPostConnection));

            Parent.Container.Register<ISolrOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    isPostConnection));

            Parent.Container.Register<ISolrReadOnlyOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    isPostConnection));
        }

        /// <summary>
        /// Startup initializing 
        /// </summary>
        public static async Task InitAsync<T>(ISolrCloudStateProvider cloudStateProvider, string collectionName, bool isPostConnection = false)
        {
            if (cloudStateProvider == null)
                throw new ArgumentNullException("cloudStateProvider");
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException("collectionName");

            await EnsureRegistrationAsync(cloudStateProvider);

            if (!Collections.Add(collectionName))
                return;

            Parent.Container.Register<ISolrBasicOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName,
                    isPostConnection));

            Parent.Container.Register<ISolrBasicReadOnlyOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName,
                    isPostConnection));

            Parent.Container.Register<ISolrOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName,
                    isPostConnection));

            Parent.Container.Register<ISolrReadOnlyOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName,
                    isPostConnection));
        }

        /// <summary>
        /// Ensures registrations and initializing
        /// </summary>
        private static async Task EnsureRegistrationAsync(ISolrCloudStateProvider cloudStateProvider)
        {
            if (Providers.Count == 0)
                Parent.Container.Register<ISolrOperationsProvider>(c => {
                    var httpWebFactory = c.GetInstance<IHttpWebRequestFactory>();
                    return new OperationsProvider(httpWebFactory);
                });

            if (Providers.ContainsKey(cloudStateProvider.Key))
                return;
            await cloudStateProvider.InitAsync();
            Providers.Add(cloudStateProvider.Key, cloudStateProvider);
            Parent.Container.Register(cloudStateProvider.Key, container => cloudStateProvider);
        }

        private class OperationsProvider : ISolrOperationsProvider
        {
            public IHttpWebRequestFactory WebRequestFactory
            {
                get; set;
            }

            internal OperationsProvider(IHttpWebRequestFactory webRequestFactory)
            {
                WebRequestFactory = webRequestFactory;
            }

            public ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool isPostConnection = false)
            {
                return SolrNet.GetBasicServer<T>(url, isPostConnection, WebRequestFactory);
            }

            public ISolrOperations<T> GetOperations<T>(string url, bool isPostConnection = false)
            {
                return SolrNet.GetServer<T>(url, isPostConnection, WebRequestFactory);
            }
        }
    }
}
