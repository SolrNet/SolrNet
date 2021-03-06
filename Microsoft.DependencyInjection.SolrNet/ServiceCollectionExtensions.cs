using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
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
using SolrNet.Microsoft.DependencyInjection;
using SolrNet.Schema;

namespace SolrNet
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Method to deal with adding a basic solr core instance.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="url">The url for the solr core.</param>
        /// <param name="setupAction">Allow for custom headers to be injected.</param>
        /// <returns>The dependency injection service.</returns>
        public static IServiceCollection AddSolrNet(this IServiceCollection services, string url,
            Action<SolrNetOptions> setupAction = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            return AddSolrNet(services, sp => url, setupAction);
        }
        
        /// <summary>
        /// Method to deal with adding a basic solr core instance.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="urlRetriever">The function to retrieve a url for the solr core.</param>
        /// <param name="setupAction">Allow for custom headers to be injected.</param>
        /// <returns>The dependency injection service.</returns>
        public static IServiceCollection AddSolrNet(this IServiceCollection services,
            Func<IServiceProvider, string> urlRetriever,
            Action<SolrNetOptions> setupAction = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (urlRetriever == null) throw new ArgumentNullException(nameof(urlRetriever));
            if (AddedGeneralDi(services)) throw new InvalidOperationException("Only one non-typed Solr Core can be added, which needs to be called before AddSolrNet<>().");

            return BuildSolrNet(services, urlRetriever, setupAction);
        }
        
        /// <summary>
        /// Method to deal with adding a second core into Microsoft's dependency injection system.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="url">The url for the second core.</param>
        /// <param name="setupAction">Allow for custom headers to be injected.</param>
        /// <typeparam name="TModel">The type of model that should be used for this core.</typeparam>
        /// <returns>The dependency injection service.</returns>
        public static IServiceCollection AddSolrNet<TModel>(this IServiceCollection services, string url,
            Action<SolrNetOptions> setupAction = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            return AddSolrNet<TModel>(services, sp => url, setupAction);
        }

        /// <summary>
        /// Method to deal with adding a second core into Microsoft's dependency injection system.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="urlRetriever">The function to retrieve a url for the second core.</param>
        /// <param name="setupAction">Allow for custom headers to be injected.</param>
        /// <typeparam name="TModel">The type of model that should be used for this core.</typeparam>
        /// <returns>The dependency injection service.</returns>
        public static IServiceCollection AddSolrNet<TModel>(this IServiceCollection services,
            Func<IServiceProvider, string> urlRetriever,
            Action<SolrNetOptions> setupAction = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (urlRetriever == null) throw new ArgumentNullException(nameof(urlRetriever));
            if (AddedDi<TModel>(services)) throw new InvalidOperationException($"SolrNet was already added for model of type {typeof(TModel).Name}");

            services = BuildSolrNet(services, urlRetriever, setupAction);

            services.AddSingleton(typeof(ISolrInjectedConnection<TModel>), serviceProvider =>
            {
                // Set custom http client setting given by user at initialization for specific solr core.
                var autoSolrConnection = CreateAutoSolrConnection(serviceProvider, urlRetriever, setupAction);
                return new BasicInjectionConnection<TModel>(autoSolrConnection);
            });
            return services;
        }

        /// <summary>
        /// Method adds the basic solr net DI to deal with multiple cores.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="urlRetriever">The function that retrieves url to be built from.</param>
        /// <param name="setupAction">The setup action that should be used for injection purposes.</param>
        /// <returns></returns>
        private static IServiceCollection BuildSolrNet(IServiceCollection services,
            Func<IServiceProvider, string> urlRetriever, Action<SolrNetOptions> setupAction)
        {
            if (AddedGeneralDi(services)) return services;
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

            // Bind single type to a single url, prevent breaking existing functionality
            services.AddSingleton<ISolrConnection>(s => CreateAutoSolrConnection(s, urlRetriever, setupAction));

            services.AddTransient(typeof(ISolrInjectedConnection<>), typeof(BasicInjectionConnection<>));
            services.AddTransient(typeof(ISolrQueryExecuter<>), typeof(SolrInjectionQueryExecuter<>));
            services.AddTransient(typeof(ISolrBasicOperations<>), typeof(SolrInjectionBasicServer<>));
            services.AddTransient(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrInjectionBasicServer<>));
            services.AddScoped(typeof(ISolrOperations<>), typeof(SolrInjectionServer<>));
            services.AddTransient(typeof(ISolrReadOnlyOperations<>), typeof(SolrInjectionServer<>));

            return services;
        }

        private static bool AddedGeneralDi(IServiceCollection services)
        {
            return services.Any(s => s.ServiceType == typeof(IReadOnlyMappingManager));
        }

        private static bool AddedDi<TModel>(IServiceCollection services)
        {
            return services.Any(s => s.ServiceType == typeof(ISolrInjectedConnection<TModel>));
        }

        private static ISolrConnection CreateAutoSolrConnection(IServiceProvider serviceProvider,
            Func<IServiceProvider, string> urlRetriever, Action<SolrNetOptions> setupAction)
        {
            var solrUrl = urlRetriever(serviceProvider);
            if (string.IsNullOrWhiteSpace(solrUrl)) throw new ArgumentNullException(nameof(solrUrl));

            var connection = new AutoSolrConnection(solrUrl);
            if (setupAction == null) return connection;

            // Allow for custom headers to be injected.
            var options = new SolrNetOptions(connection.HttpClient);
            setupAction(options);
            return connection;
        }
    }
}
