using SolrNet;
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
using System;
using System.Collections.Generic;

namespace SimpleInjector.Integration.SolrNet
{
    public class SolrNetConfiguration
    {
        private readonly Container _Container;

        public SolrNetConfiguration(Container container)
        {
            _Container = container;
        }

        /// <summary>
        /// Configures SolrNet dependencies for this container.
        /// </summary>
        public void ConfigureContainer()
        {
            _Container.Register<IReadOnlyMappingManager, AllPropertiesMappingManager>();
            _Container.RegisterDecorator<IReadOnlyMappingManager, MemoizingMappingManager>();
            _Container.Register(typeof(ISolrDocumentActivator<>), typeof(SolrDocumentActivator<>));
            _Container.Register<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
            _Container.Register<IMappingValidator, MappingValidator>();
            _Container.Register(typeof(ISolrOperations<>), typeof(SolrServer<>));

            // parsers
            _Container.Register<ISolrDocumentResponseParser<Dictionary<string, object>>, SolrDictionaryDocumentResponseParser>();
            _Container.RegisterConditional(typeof(ISolrDocumentResponseParser<>), typeof(SolrDocumentResponseParser<>)
                , x => x.Handled == false);
            _Container.Register(typeof(ISolrAbstractResponseParser<>), typeof(DefaultResponseParser<>));
            _Container.RegisterCollection(typeof(ISolrAbstractResponseParser<>), new[] { typeof(ISolrAbstractResponseParser<>).Assembly });
            _Container.Register<ISolrHeaderResponseParser, HeaderResponseParser<string>>();
            _Container.Register<ISolrExtractResponseParser, ExtractResponseParser>();
            _Container.Register(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>), typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
            _Container.Register<ISolrFieldParser, DefaultFieldParser>();
            _Container.Register<ISolrSchemaParser, SolrSchemaParser>();
            _Container.Register<ISolrDIHStatusParser, SolrDIHStatusParser>();
            _Container.Register<ISolrStatusResponseParser, SolrStatusResponseParser>();
            _Container.Register<ISolrCoreAdmin, SolrCoreAdmin>();

            // validation rules
            _Container.RegisterCollection(typeof(IValidationRule), new[] {
                typeof (MappedPropertiesIsInSolrSchemaRule),
                typeof (RequiredFieldsAreMappedRule),
                typeof (UniqueKeyMatchesMappingRule),
                typeof(MultivaluedMappedToCollectionRule),
            });

            // serializers
            _Container.Register<ISolrDocumentSerializer<Dictionary<string, object>>, SolrDictionarySerializer>();
            _Container.RegisterConditional(typeof(ISolrDocumentSerializer<>), typeof(SolrDocumentSerializer<>)
                , x => x.Handled == false);
            _Container.Register<ISolrFieldSerializer, DefaultFieldSerializer>();
            _Container.Register<ISolrQuerySerializer, DefaultQuerySerializer>();
            _Container.Register<ISolrFacetQuerySerializer, DefaultFacetQuerySerializer>();

            // internal factories
            _Container.Register<Factories.BasicOperationFactory>();
            _Container.Register<Factories.QueryExecuterFactory>();
        }

        /// <summary>
        /// Registers a core
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="connectionUrl">Connection URL of the core to register</param>
        public void RegisterCore<TEntity>(string connectionUrl)
        {
            if (string.IsNullOrEmpty(connectionUrl))
            {
                throw new ArgumentNullException(connectionUrl, nameof(connectionUrl));
            }

            var core = new SolrCore();
            core.DocumentType = typeof(TEntity);
            core.ConnectionUrl = connectionUrl;

            RegisterSolrQueryExecuter(core: core);
            RegisterBasicOperations(core: core);
        }

        /// <summary>
        /// Registers a connection builder for all cores.
        /// </summary>
        /// <typeparam name="TConnectionBuilder">Connection builder implementation</typeparam>
        public void RegisterCores<TConnectionBuilder>() where TConnectionBuilder : class, IConnectionBuilder, new()
        {
            _Container.Register<IConnectionBuilder, TConnectionBuilder>();
            _Container.ResolveUnregisteredType += ResolveQueryExecuterEventHandler;
            _Container.ResolveUnregisteredType += ResolveBasicOperationEventHandler;
            _Container.ResolveUnregisteredType += ResolveReadOnlyOperationEventHandler;
        }

        private void RegisterSolrQueryExecuter(SolrCore core)
        {
            var ISolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);

            _Container.Register(serviceType: ISolrQueryExecuter, instanceCreator: () =>
            {
                var factory = _Container.GetInstance<Factories.QueryExecuterFactory>();

                return factory.Create(core: core);
            });
        }

        private void RegisterBasicOperations(SolrCore core)
        {
            var ISolrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(core.DocumentType);
            var ISolrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(core.DocumentType);

            var instanceCreator = new Func<object>(() =>
            {
                var factory = _Container.GetInstance<Factories.BasicOperationFactory>();

                return factory.Create(core: core);
            });

            _Container.Register(serviceType: ISolrBasicOperations, instanceCreator: instanceCreator);
            _Container.Register(serviceType: ISolrBasicReadOnlyOperations, instanceCreator: instanceCreator);
        }

        private void ResolveQueryExecuterEventHandler(object sender, UnregisteredTypeEventArgs args)
        {
            if (args.UnregisteredServiceType.IsGenericType && args.UnregisteredServiceType.GetGenericTypeDefinition() == typeof(ISolrQueryExecuter<>))
            {
                var docType = args.UnregisteredServiceType.GetGenericArguments()[0];

                var factory = _Container.GetInstance<Factories.QueryExecuterFactory>();

                var connectionBuilder = _Container.GetInstance<IConnectionBuilder>();

                var core = new SolrCore();

                core.ConnectionUrl = connectionBuilder.Build(docType);
                core.DocumentType = docType;

                args.Register(() =>
                {
                    return factory.Create(core: core);
                });
            }
        }

        private void ResolveBasicOperationEventHandler(object sender, UnregisteredTypeEventArgs args)
        {
            if (args.UnregisteredServiceType.IsGenericType && args.UnregisteredServiceType.GetGenericTypeDefinition() == typeof(ISolrBasicOperations<>))
            {
                var docType = args.UnregisteredServiceType.GetGenericArguments()[0];

                var factory = _Container.GetInstance<Factories.BasicOperationFactory>();
                var connectionBuilder = _Container.GetInstance<IConnectionBuilder>();
                var core = new SolrCore();
                core.ConnectionUrl = connectionBuilder.Build(docType);
                core.DocumentType = docType;

                args.Register(() =>
                {
                    return factory.Create(core: core);
                });
            }
        }

        private void ResolveReadOnlyOperationEventHandler(object sender, UnregisteredTypeEventArgs args)
        {
            if (args.UnregisteredServiceType.IsGenericType && args.UnregisteredServiceType.GetGenericTypeDefinition() == typeof(ISolrBasicReadOnlyOperations<>))
            {
                var docType = args.UnregisteredServiceType.GetGenericArguments()[0];

                var outputType = typeof(ISolrBasicOperations<>).MakeGenericType(docType);

                args.Register(() =>
                {
                    return _Container.GetInstance(serviceType: outputType);
                });
            }
        }
    }
}
