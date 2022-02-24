using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SolrNet.Cloud.ZooKeeperClient;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;

namespace SolrNet.Cloud
{
    /// <summary>
    /// Provides extension methods to register SolrNet.Cloud via Microsoft .NET Core dependency injection system.
    /// </summary>
    public static class ServiceCollectionExtensionsSolrCloud
    {
        /// <summary>
        /// Registers SolrNet.Cloud via Microsoft .NET Core dependency injection system.
        /// The async version <see cref="AddSolrNetCloudAsync{TModel}(IServiceCollection, string, string)"/>
        /// of this method is preferred whenever possible!
        /// </summary>
        /// <typeparam name="TModel">The type of the model to register.</typeparam>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="zooKeeperUrl">The ZooKeeper URL for SolrCloud.</param>
        /// <param name="collection">The collection name.</param>
        /// <returns>The dependency injection service.</returns>
        public static IServiceCollection AddSolrNetCloud<TModel>(this IServiceCollection services, string zooKeeperUrl, string collection)
        {
            return AddSolrNetCloudAsync<TModel>(services, zooKeeperUrl, collection).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Registers SolrNet.Cloud via Microsoft .NET Core dependency injection system.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to register.</typeparam>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="zooKeeperUrl">The ZooKeeper URL for SolrCloud.</param>
        /// <param name="collection">The collection name.</param>
        /// <returns>The dependency injection service.</returns>
        public static async Task<IServiceCollection> AddSolrNetCloudAsync<TModel>(this IServiceCollection services, string zooKeeperUrl, string collection)
        {
            if (services == null) 
                throw new ArgumentNullException(nameof(services));            
            
            if (string.IsNullOrWhiteSpace(zooKeeperUrl)) 
                throw new ArgumentNullException(nameof(zooKeeperUrl));            
            
            if (string.IsNullOrEmpty(collection))
                throw new ArgumentNullException(nameof(collection));
            
            if (IsTypeAdded<TModel>(services))
                throw new InvalidOperationException($"SolrNet was already added for model of type {typeof(TModel).Name}");

            AddCommon(services);
            await AddCloud<TModel>(services, zooKeeperUrl, collection, false);
            return services;
        }

        /// <summary>
        /// Adds common SolrNet types.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        private static void AddCommon(IServiceCollection services)
        {
            if (IsAddedCommon(services))
                return;

            services.AddSingleton<IReadOnlyMappingManager>(new MemoizingMappingManager(new AttributesMappingManager()));
            services.AddTransient<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
            services.AddTransient<ISolrFieldParser, DefaultFieldParser>();
            services.AddTransient(typeof(ISolrDocumentActivator<>), typeof(SolrDocumentActivator<>));
            services.AddTransient(typeof(ISolrDocumentResponseParser<>), typeof(SolrDocumentResponseParser<>));
            services.AddTransient<ISolrDocumentResponseParser<Dictionary<string, object>>, SolrDictionaryDocumentResponseParser>();
            services.AddTransient<ISolrFieldSerializer, DefaultFieldSerializer>();
            services.AddTransient<ISolrQuerySerializer, DefaultQuerySerializer>();
            services.AddTransient<ISolrFacetQuerySerializer, DefaultFacetQuerySerializer>();
            services.AddTransient(typeof(ISolrAbstractResponseParser<>), typeof(DefaultResponseParser<>));
            services.AddTransient<ISolrHeaderResponseParser, HeaderResponseParser<string>>();
            services.AddTransient<ISolrExtractResponseParser, ExtractResponseParser>();
            foreach (var p in new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
                typeof(MultivaluedMappedToCollectionRule),
            })
            services.AddTransient(typeof(IValidationRule), p);
            services.AddTransient(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>), typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
            services.AddTransient(typeof(ISolrDocumentSerializer<>), typeof(SolrDocumentSerializer<>));
            services.AddTransient<ISolrDocumentSerializer<Dictionary<string, object>>, SolrDictionarySerializer>();
            services.AddTransient<ISolrSchemaParser, SolrSchemaParser>();
            services.AddTransient<ISolrDIHStatusParser, SolrDIHStatusParser>();
            services.AddTransient<IMappingValidator, MappingValidator>();
        }

        private static bool IsAddedCommon(IServiceCollection services)
        {
            return services.Any(s => s.ServiceType == typeof(IReadOnlyMappingManager));
        }

        private static bool IsTypeAdded<TModel>(IServiceCollection services)
        {
            return services.Any(s => s.ServiceType == typeof(ISolrBasicOperations<TModel>));
        }

        private static async Task<ISolrCloudStateProvider> EnsureCloudRegistration(IServiceCollection services, string zooKeeperUrl)
        {
            var sp = services.BuildServiceProvider();
            
            var op = sp.GetService<ISolrOperationsProvider>();
            if (op == null)
                services.AddSingleton(typeof(ISolrOperationsProvider), new OperationsProvider());
            
            var cloudStateProvider = sp.GetServices<ISolrCloudStateProvider>()
                .FirstOrDefault(o => o.Key == zooKeeperUrl);
            if (cloudStateProvider != null)
                return cloudStateProvider;

            cloudStateProvider = new SolrCloudStateProvider(zooKeeperUrl);
            await cloudStateProvider.InitAsync();
            services.AddSingleton(typeof(ISolrCloudStateProvider), cloudStateProvider);

            return cloudStateProvider;
        }

        private static async Task AddCloud<T>(IServiceCollection services, string zooKeeperUrl, string collection, bool isPostConnection = false)
        {
            var cloudStateProvider = await EnsureCloudRegistration(services, zooKeeperUrl);
            var sp = services.BuildServiceProvider();
            var op = sp.GetService<ISolrOperationsProvider>();

            services.AddScoped(typeof(ISolrBasicOperations<T>),
                factory => new SolrCloudBasicOperations<T>(
                    //cloudStateProvider,
                    //sp.GetServices<ISolrCloudStateProvider>(),
                    sp.GetServices<ISolrCloudStateProvider>().FirstOrDefault(o => o.Key == zooKeeperUrl),
                    //op,
                    sp.GetService<ISolrOperationsProvider>(),
                    collection,
                    isPostConnection));

            services.AddScoped(typeof(ISolrBasicReadOnlyOperations<T>),
                factory => new SolrCloudBasicOperations<T>(
                    //cloudStateProvider,
                    //sp.GetServices<ISolrCloudStateProvider>(),
                    sp.GetServices<ISolrCloudStateProvider>().FirstOrDefault(o => o.Key == zooKeeperUrl),
                    //op,
                    sp.GetService<ISolrOperationsProvider>(),
                    collection,
                    isPostConnection));

            services.AddScoped(typeof(ISolrOperations<T>),
                factory => new SolrCloudOperations<T>(
                    //cloudStateProvider,
                    //sp.GetServices<ISolrCloudStateProvider>(),
                    sp.GetServices<ISolrCloudStateProvider>().FirstOrDefault(o => o.Key == zooKeeperUrl),
                    //op,
                    sp.GetService<ISolrOperationsProvider>(),
                    collection,
                    isPostConnection));

            services.AddScoped(typeof(ISolrReadOnlyOperations<T>),
                factory => new SolrCloudOperations<T>(
                    //cloudStateProvider,
                    //sp.GetServices<ISolrCloudStateProvider>(),
                    sp.GetServices<ISolrCloudStateProvider>().FirstOrDefault(o => o.Key == zooKeeperUrl),
                    //op,
                    sp.GetService<ISolrOperationsProvider>(),
                    collection,
                    isPostConnection));
        }

        private class OperationsProvider : ISolrOperationsProvider
        {
            public ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool isPostConnection = false)
            {
                return SolrNet.GetBasicServer<T>(url, isPostConnection);
            }

            public ISolrOperations<T> GetOperations<T>(string url, bool isPostConnection = false)
            {
                return SolrNet.GetServer<T>(url, isPostConnection);
            }
        }
    }
}
