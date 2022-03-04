using System;
using System.Collections.Generic;
using System.Linq;
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
using LightInject.SolrNet;
using LightInject;
using SolrNet.Schema;
using System.Net;
using HttpWebAdapters;

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
        /// <param name="httpWebRequestFactoryFunc">Allow for injection of IHttpWebRequestFactory.</param>
        /// <returns>The dependency injection service.</returns>
        public static IServiceContainer AddSolrNet(this IServiceContainer services, string url,
            Action<SolrNetOptions> setupAction = null, 
            Func<IHttpWebRequestFactory> httpWebRequestFactoryFunc = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            return AddSolrNet(services, sp => url, setupAction, httpWebRequestFactoryFunc);
        }
        
        /// <summary>
        /// Method to deal with adding a basic solr core instance.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="urlRetriever">The function to retrieve a url for the solr core.</param>
        /// <param name="setupAction">Allow for custom headers to be injected.</param>
        /// <param name="httpWebRequestFactoryFunc">Allow for injection of IHttpWebRequestFactory.</param>
        /// <returns>The dependency injection service.</returns>
        public static IServiceContainer AddSolrNet(this IServiceContainer services,
            Func<IServiceContainer, string> urlRetriever,
            Action<SolrNetOptions> setupAction = null, 
            Func<IHttpWebRequestFactory> httpWebRequestFactoryFunc = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (urlRetriever == null) throw new ArgumentNullException(nameof(urlRetriever));
            if (AddedGeneralDi(services)) throw new InvalidOperationException("Only one non-typed Solr Core can be added, which needs to be called before AddSolrNet<>().");

            return BuildSolrNet(services, urlRetriever, setupAction, httpWebRequestFactoryFunc);
        }
        
        /// <summary>
        /// Method to deal with adding a second core into Microsoft's dependency injection system.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="url">The url for the second core.</param>
        /// <param name="setupAction">Allow for custom headers to be injected.</param>
        /// <param name="httpWebRequestFactoryFunc">Allow for injection of IHttpWebRequestFactory.</param>
        /// <typeparam name="TModel">The type of model that should be used for this core.</typeparam>
        /// <returns>The dependency injection service.</returns>
        public static IServiceContainer AddSolrNet<TModel>(this IServiceContainer services, string url,
            Action<SolrNetOptions> setupAction = null, Func<IHttpWebRequestFactory> httpWebRequestFactoryFunc = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            return AddSolrNet<TModel>(services, sp => url, setupAction, httpWebRequestFactoryFunc);
        }

        /// <summary>
        /// Method to deal with adding a second core into Microsoft's dependency injection system.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="urlRetriever">The function to retrieve a url for the second core.</param>
        /// <param name="setupAction">Allow for custom headers to be injected.</param>
        /// <param name="httpWebRequestFactoryFunc">Allow for injection of IHttpWebRequestFactory.</param>
        /// <typeparam name="TModel">The type of model that should be used for this core.</typeparam>
        /// <returns>The dependency injection service.</returns>
        public static IServiceContainer AddSolrNet<TModel>(this IServiceContainer services,
            Func<IServiceContainer, string> urlRetriever,
            Action<SolrNetOptions> setupAction = null, Func<IHttpWebRequestFactory> httpWebRequestFactoryFunc = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (urlRetriever == null) throw new ArgumentNullException(nameof(urlRetriever));
            if (AddedDi<TModel>(services)) throw new InvalidOperationException($"SolrNet was already added for model of type {typeof(TModel).Name}");

            services = BuildSolrNet(services, urlRetriever, setupAction, httpWebRequestFactoryFunc);

            services.Register<ISolrInjectedConnection<TModel>>(factory => 
            {
                var autoSolrConnection = CreateAutoSolrConnection(services, urlRetriever, setupAction, httpWebRequestFactoryFunc);
                return new BasicInjectionConnection<TModel>(autoSolrConnection);
            }, new PerRequestLifeTime());

            return services;
        }

        /// <summary>
        /// Method adds the basic solr net DI to deal with multiple cores.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="urlRetriever">The function that retrieves url to be built from.</param>
        /// <param name="setupAction">Allow for custom headers to be injected.</param>
        /// <returns></returns>
        private static IServiceContainer BuildSolrNet(IServiceContainer services,
            Func<IServiceContainer, string> urlRetriever, Action<SolrNetOptions> setupAction, Func<IHttpWebRequestFactory> httpWebRequestFactoryFunc)
        {
            if (AddedGeneralDi(services)) return services;
  
            services.RegisterInstance<IReadOnlyMappingManager>(new MemoizingMappingManager(new AttributesMappingManager()));
            services.Register<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
            services.Register<ISolrFieldParser, DefaultFieldParser>();
            services.Register(typeof(ISolrDocumentActivator<>), typeof(SolrDocumentActivator<>));
            services.Register(typeof(ISolrDocumentResponseParser<>), typeof(SolrDocumentResponseParser<>));

            services.Register<ISolrDocumentResponseParser<Dictionary<string, object>>, SolrDictionaryDocumentResponseParser>();
            services.Register<ISolrFieldSerializer, DefaultFieldSerializer>();
            services.Register<ISolrQuerySerializer, DefaultQuerySerializer>();
            services.Register<ISolrFacetQuerySerializer, DefaultFacetQuerySerializer>();
            services.Register(typeof(ISolrAbstractResponseParser<>), typeof(DefaultResponseParser<>));
            services.Register<ISolrHeaderResponseParser, HeaderResponseParser<string>>();
            services.Register<ISolrExtractResponseParser, ExtractResponseParser>();

            foreach(var validationRule in new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
                typeof(MultivaluedMappedToCollectionRule),
            })
            {
                services.Register(typeof(IValidationRule), validationRule);
            }
                
            services.Register(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>), typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
            services.Register(typeof(ISolrDocumentSerializer<>), typeof(SolrDocumentSerializer<>));
            services.Register<ISolrDocumentSerializer<Dictionary<string, object>>, SolrDictionarySerializer>();

            services.Register<ISolrSchemaParser, SolrSchemaParser>();
            services.Register<ISolrDIHStatusParser, SolrDIHStatusParser>();
            services.Register<IMappingValidator, MappingValidator>();

            // Bind single type to a single url, prevent breaking existing functionality
            services.Register<ISolrConnection>(s => CreateAutoSolrConnection(services, urlRetriever, setupAction, httpWebRequestFactoryFunc));

            services.Register(typeof(ISolrInjectedConnection<>), typeof(BasicInjectionConnection<>));
            services.Register(typeof(ISolrQueryExecuter<>), typeof(SolrInjectionQueryExecuter<>)); 
            services.Register(typeof(ISolrBasicOperations<>), typeof(SolrInjectionBasicServer<>));
            services.Register(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrInjectionBasicServer<>));
            services.Register(typeof(ISolrOperations<>), typeof(SolrInjectionServer<>));
            services.Register(typeof(ISolrReadOnlyOperations<>), typeof(SolrInjectionServer<>));

            return services;
        }

        private static bool AddedGeneralDi(IServiceContainer services)
        {
            return services.AvailableServices.Any(s => s.ServiceType == typeof(IReadOnlyMappingManager));
        }

        private static bool AddedDi<TModel>(IServiceContainer services)
        {
            return services.AvailableServices.Any(s => s.ServiceType == typeof(ISolrInjectedConnection<TModel>));
        }

        private static ISolrConnection CreateAutoSolrConnection(IServiceContainer serviceProvider,
            Func<IServiceContainer, string> urlRetriever, Action<SolrNetOptions> setupAction, Func<IHttpWebRequestFactory> httpWebRequestFactoryFunc)
        {
            var solrUrl = urlRetriever(serviceProvider);
            if (string.IsNullOrWhiteSpace(solrUrl)) throw new ArgumentNullException(nameof(solrUrl));

            var httpWebRequestFactory = httpWebRequestFactoryFunc != null ? httpWebRequestFactoryFunc() :new HttpWebRequestFactory();

            // Use constructor that enables both async and sync web requests, default ICredentials, they can be injected by callee via SolrNetOptions.
            var connection = new AutoSolrConnection(solrUrl, default(ICredentials), httpWebRequestFactory);
            if (setupAction == null) return connection;

            // Allow for custom headers to be injected.
            var options = new SolrNetOptions(connection.HttpClient);
            setupAction(options);
            return connection;
        }
    }
}
