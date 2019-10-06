using System;
using System.Collections.Generic;
using SolrNet.Utils;
using System.Threading.Tasks;
using Autofac;
using SolrNet.Cloud;
using SolrNet;
using SolrNet.Cloud.ZooKeeperClient;
using Autofac.Core;

namespace AutofacContrib.SolrNetCloud
{
    /// <summary>
    /// Configures SolrNet in an Autofac container
    /// </summary>
    public class SolrNetCloudModule : Module
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
        /// Static Constructor
        /// </summary>
        static SolrNetCloudModule()
        {
            Collections = new HashSet<string>();
            Providers = new Dictionary<string, ISolrCloudStateProvider>(StringComparer.OrdinalIgnoreCase);
        }
        public SolrNetCloudModule(string zookeeperConnection, bool isPostConnection = false) 
            :this(new SolrCloudStateProvider(zookeeperConnection), isPostConnection)
        {}
        public SolrNetCloudModule(string zookeeperConnection, string collectionName, bool isPostConnection = false)
            : this(new SolrCloudStateProvider(zookeeperConnection), collectionName, isPostConnection)
        { }
        public SolrNetCloudModule(ISolrCloudStateProvider cloudStateProvider, string collectionName, bool isPostConnection = false)
            : this(cloudStateProvider, isPostConnection)
        {
            CollectionName = collectionName;
        }
        public SolrNetCloudModule(ISolrCloudStateProvider cloudStateProvider, bool isPostConnection = false)
        {
            CloudStateProvider = cloudStateProvider;
            IsPostConnection = isPostConnection;
        }

        private ISolrCloudStateProvider CloudStateProvider { get; }
        private string CollectionName { get; }
        private bool IsPostConnection { get; }


        protected override void Load(ContainerBuilder builder)
        {
            EnsureRegistration(builder, CloudStateProvider);

            if (!string.IsNullOrEmpty(CollectionName))
            {
                if (!Collections.Add(CollectionName))
                    return;
                builder.RegisterCollection(CloudStateProvider, CollectionName, IsPostConnection);
            }else
            {
                if (!Collections.Add(string.Empty))
                    return;
                builder.RegisterCollection(CloudStateProvider, IsPostConnection);
            }
        }

        private static void EnsureRegistration(ContainerBuilder builder, ISolrCloudStateProvider cloudStateProvider)
        {
            builder.RegisterType<OperationsProvider>()
                .As<ISolrOperationsProvider>()
                .IfNotRegistered(typeof(ISolrOperationsProvider));

            if (Providers.ContainsKey(cloudStateProvider.Key))
                return;

            cloudStateProvider.InitAsync().GetAwaiter().GetResult();

            Providers.Add(cloudStateProvider.Key, cloudStateProvider);
            builder.Register(c => cloudStateProvider).Keyed<ISolrCloudStateProvider>(cloudStateProvider.Key);
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

    public static class ContainerBuilderExtensions
    {
        public static void RegisterCollection(this ContainerBuilder builder, ISolrCloudStateProvider cloudStateProvider, string collectionName, bool isPostConnection = false)
        {
            if (cloudStateProvider == null)
                throw new ArgumentNullException(nameof(cloudStateProvider));
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            var cloudStateProviderParam = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(ISolrCloudStateProvider) && pi.Name == nameof(cloudStateProvider),
                (pi, ctx) => ctx.ResolveKeyed<ISolrCloudStateProvider>(cloudStateProvider.Key));
            var operationsProviderParam = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(ISolrOperationsProvider),
                (pi, ctx) => ctx.Resolve<ISolrOperationsProvider>());
            var collectionNameParam = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == nameof(collectionName),
                (pi, ctx) => collectionName);
            var isPostConnectionParam = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(bool) && pi.Name == nameof(isPostConnection),
                (pi, ctx) => isPostConnection);

            var solrOperationsParams = new Parameter[] { cloudStateProviderParam, operationsProviderParam, collectionNameParam, isPostConnectionParam };

            builder.RegisterGeneric(typeof(SolrCloudBasicOperations<>))
                .As(typeof(ISolrBasicOperations<>), typeof(ISolrBasicReadOnlyOperations<>))
                .WithParameters(solrOperationsParams);

            builder.RegisterGeneric(typeof(SolrCloudOperations<>))
                .As(typeof(ISolrOperations<>), typeof(ISolrReadOnlyOperations<>))
                .WithParameters(solrOperationsParams);
        }

        public static void RegisterCollection(this ContainerBuilder builder, ISolrCloudStateProvider cloudStateProvider, bool isPostConnection = false)
        {
            if (cloudStateProvider == null)
                throw new ArgumentNullException(nameof(cloudStateProvider));

            var cloudStateProviderParam = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(ISolrCloudStateProvider) && pi.Name == nameof(cloudStateProvider),
                (pi, ctx) => ctx.ResolveKeyed<ISolrCloudStateProvider>(cloudStateProvider.Key));
            var operationsProviderParam = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(ISolrOperationsProvider),
                (pi, ctx) => ctx.Resolve<ISolrOperationsProvider>());
            var isPostConnectionParam = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(bool) && pi.Name == nameof(isPostConnection),
                (pi, ctx) => isPostConnection);

            var solrOperationsParams = new Parameter[] { cloudStateProviderParam, operationsProviderParam, isPostConnectionParam };

            builder.RegisterGeneric(typeof(SolrCloudBasicOperations<>))
                .As(typeof(ISolrBasicOperations<>), typeof(ISolrBasicReadOnlyOperations<>))
                .WithParameters(solrOperationsParams);

            builder.RegisterGeneric(typeof(SolrCloudOperations<>))
                .As(typeof(ISolrOperations<>), typeof(ISolrReadOnlyOperations<>))
                .WithParameters(solrOperationsParams);
        }
    }
}

