using System;
using Microsoft.Practices.Unity;
using SolrNet;
using SolrNet.Cloud;

namespace Unity.SolrNetCloudIntegration
{
    /// <summary>
    /// Unity container configuration
    /// </summary>
    public class SolrNetContainerConfiguration
    {
        /// <summary>
        /// Returns configured unity container
        /// </summary>
        public IUnityContainer ConfigureContainer(ISolrCloudStateProvider cloudStateProvider, IUnityContainer container) {
            if (cloudStateProvider == null)
                throw new ArgumentNullException("cloudStateProvider");
            if (container == null)
                throw new ArgumentNullException("container");
            if (container.IsRegistered<ISolrCloudStateProvider>(cloudStateProvider.Key))
                return container;
            cloudStateProvider.Init();
            foreach (var collection in cloudStateProvider.GetCloudState().Collections.Keys) {
                if (!container.IsRegistered<ISolrCloudStateProvider>())
                    RegisterFirstCollection(cloudStateProvider, container);
                RegisterCollection(cloudStateProvider, collection, container);
            }
            container.RegisterInstance(cloudStateProvider.Key, cloudStateProvider);
            if (!container.IsRegistered<ISolrOperationsProvider>())
                container.RegisterInstance<ISolrOperationsProvider>(new OperationsProvider());
            return container;
        }

        /// <summary>
        /// Registers collection
        /// </summary>
        private static void RegisterCollection(ISolrCloudStateProvider cloudStateProvider, string collection, IUnityContainer container) {
            var injection = new InjectionConstructor(
                    new ResolvedParameter<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    new ResolvedParameter<ISolrOperationsProvider>(),
                    collection);
            container.RegisterType(typeof(ISolrBasicOperations<>), typeof(SolrCloudBasicOperations<>), collection, injection);
            container.RegisterType(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrCloudBasicOperations<>), collection, injection);
            container.RegisterType(typeof(ISolrOperations<>), typeof(SolrCloudOperations<>), collection, injection);
            container.RegisterType(typeof(ISolrReadOnlyOperations<>), typeof(SolrCloudOperations<>), collection, injection);
        }

        /// <summary>
        /// Registers first collection
        /// </summary>
        private static void RegisterFirstCollection(ISolrCloudStateProvider cloudStateProvider, IUnityContainer container) {
            var injection = new InjectionConstructor(
                    new ResolvedParameter<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    new ResolvedParameter<ISolrOperationsProvider>());
            container.RegisterType(typeof(ISolrBasicOperations<>), typeof(SolrCloudBasicOperations<>), injection);
            container.RegisterType(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrCloudBasicOperations<>), injection);
            container.RegisterType(typeof(ISolrOperations<>), typeof(SolrCloudOperations<>), injection);
            container.RegisterType(typeof(ISolrReadOnlyOperations<>), typeof(SolrCloudOperations<>), injection);
        }


        private class OperationsProvider : ISolrOperationsProvider
        {
            public ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool isPostConnection = false)
            {
                return SolrNet.SolrNet.GetBasicServer<T>(url, isPostConnection);
            }

            public ISolrOperations<T> GetOperations<T>(string url, bool isPostConnection = false)
            {
                return SolrNet.SolrNet.GetServer<T>(url, isPostConnection);
            }
        }
    }
}