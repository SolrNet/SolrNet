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
using System;
using System.Collections.Generic;
using System.Linq;
namespace SolrNet
{
    public static class ServiceCollectionExtensions
    {
        private static bool addedGeneralDI = false;
        public static IServiceCollection AddSolrNet(this IServiceCollection services, string url)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return services.AddSolrNet(new SolrCore(null, null, url), null);
        }

        public static IServiceCollection AddSolrNet(this IServiceCollection services, string url, Action<SolrNetOptions> setupAction) => AddSolrNet(services, new SolrCore(null, null, url), setupAction);

        private static IServiceCollection AddSolrNet(this IServiceCollection services, SolrCore core, Action<SolrNetOptions> setupAction)
        {
            if (core == null) return services;
            return BuildSolrNet(services, core.Url, setupAction);
        }

        /// <summary>
        /// Method to deal with adding a second core into Microsoft's dependency injection system.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="url">The url for the second core.</param>
        /// <typeparam name="TModel">The type of model that should be used for this core.</typeparam>
        /// <returns>The dependency injection service.</returns>
        public static IServiceCollection AddSolrNet<TModel>(this IServiceCollection services, string url)
        {
            return AddSolrNet<TModel>(services, url, null);
        }

        /// <summary>
        /// Method to deal with adding a second core into Microsoft's dependency injection system.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="url">The url for the second core.</param>
        /// <param name="setupAction">allow for custom headers to be injected.</param>
        /// <typeparam name="TModel">The type of model that should be used for this core.</typeparam>
        /// <returns>The dependency injection service.</returns>
        public static IServiceCollection AddSolrNet<TModel>(this IServiceCollection services, string url, Action<SolrNetOptions> setupAction)
        {
            services = BuildSolrNet(services, url, setupAction);
            var connection = new BasicInjectionConnection<TModel>(new AutoSolrConnection(url));
            services.AddTransient(typeof(ISolrInjectedConnection<TModel>), (service) => connection);
            return services;
        }

        /// <summary>
        /// Method is mean to add in the basic solr net DI to deal with multiple cores.
        /// </summary>
        /// <param name="services">The dependency injection service.</param>
        /// <param name="url">The url to be built from.</param>
        /// <param name="setupAction">The setup action that should be used for injection purposes.</param>
        /// <returns></returns>
        private static IServiceCollection BuildSolrNet(IServiceCollection services, string url, Action<SolrNetOptions> setupAction) {
            if (addedGeneralDI) return services;
            addedGeneralDI = true;

            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IReadOnlyMappingManager, AttributesMappingManager>();
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

            var connection = new AutoSolrConnection(url);
            //Bind single type to a single url, prevent breaking existing functionality
            services.AddSingleton<ISolrConnection>(connection);

            services.AddTransient(typeof(ISolrInjectedConnection<>), typeof(BasicInjectionConnection<>));
            services.AddTransient(typeof(ISolrQueryExecuter<>), typeof(SolrInjectionQueryExecuter<>));
            services.AddTransient(typeof(ISolrBasicOperations<>), typeof(SolrInjectionBasicServer<>));
            services.AddTransient(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrInjectionBasicServer<>));
            services.AddScoped(typeof(ISolrOperations<>), typeof(SolrInjectionServer<>));
            services.AddTransient(typeof(ISolrReadOnlyOperations<>), typeof(SolrInjectionServer<>));

            
            if (setupAction != null)
            {
                var options = new SolrNetOptions(connection.HttpClient);
                //allow for custom headers to be injected.
                setupAction(options);
            }

            return services;
        }
    }
}