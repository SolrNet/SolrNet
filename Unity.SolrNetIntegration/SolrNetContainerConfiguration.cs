using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity;
using SolrNet;
using SolrNet.Exceptions;
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
using SolrNet.Utils;
using Unity.SolrNetIntegration.Config;

namespace Unity.SolrNetIntegration {
    public class SolrNetContainerConfiguration {
        public IUnityContainer ConfigureContainer(SolrServers solrServers, IUnityContainer container) {
            container.RegisterType<IReadOnlyMappingManager, MemoizingMappingManager>(new InjectionConstructor(new ResolvedParameter(typeof (AttributesMappingManager))));
            container.RegisterType(typeof (ISolrDocumentActivator<>), typeof (SolrDocumentActivator<>));
            container.RegisterType(typeof (ISolrQueryExecuter<>), typeof (SolrQueryExecuter<>));
            container.RegisterType<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
            container.RegisterType<IMappingValidator, MappingValidator>();
            RegisterParsers(container);
            RegisterValidationRules(container);
            RegisterSerializers(container);

            AddCoresFromConfig(solrServers, container);

            return container;
        }

        private void RegisterValidationRules(IUnityContainer container) {
            var validationRules = new[] {
                typeof (MappedPropertiesIsInSolrSchemaRule),
                typeof (RequiredFieldsAreMappedRule),
                typeof (UniqueKeyMatchesMappingRule),
            };

            foreach (var validationRule in validationRules) {
                container.RegisterType(typeof (IValidationRule), validationRule);
            }
        }

        private void RegisterSerializers(IUnityContainer container) {
            container.RegisterType(typeof (ISolrDocumentSerializer<>), typeof (SolrDocumentSerializer<>));
            container.RegisterType(typeof (ISolrDocumentSerializer<Dictionary<string, object>>), typeof (SolrDictionarySerializer));
            container.RegisterType<ISolrFieldSerializer, DefaultFieldSerializer>();
            container.RegisterType<ISolrQuerySerializer, DefaultQuerySerializer>();
            container.RegisterType<ISolrFacetQuerySerializer, DefaultFacetQuerySerializer>();
        }

        private void RegisterParsers(IUnityContainer container) {
            container.RegisterType(typeof (ISolrDocumentResponseParser<>), typeof (SolrDocumentResponseParser<>));
            container.RegisterType<ISolrDocumentResponseParser<Dictionary<string, object>>, SolrDictionaryDocumentResponseParser>();
            container.RegisterType(typeof (ISolrAbstractResponseParser<>), typeof (DefaultResponseParser<>));

            container.RegisterType<ISolrHeaderResponseParser, HeaderResponseParser<string>>();
            container.RegisterType<ISolrExtractResponseParser, ExtractResponseParser>();
            container.RegisterType(typeof (ISolrMoreLikeThisHandlerQueryResultsParser<>), typeof (SolrMoreLikeThisHandlerQueryResultsParser<>));
            container.RegisterType<ISolrFieldParser, DefaultFieldParser>();
            container.RegisterType<ISolrSchemaParser, SolrSchemaParser>();
            container.RegisterType<ISolrDIHStatusParser, SolrDIHStatusParser>();
        }

        private void RegisterCore(SolrCore core, IUnityContainer container) {
            GetCoreConnectionId(core);
            container.RegisterType<ISolrConnection, SolrConnection>(GetCoreConnectionId(core), new InjectionConstructor(core.Url));
            RegisterSolrQueryExecuter(core, container);
            RegisterBasicOperations(core, container);
            RegisterSolrOperations(core, container);
        }

        private static void RegisterSolrOperations(SolrCore core, IUnityContainer container) {
            var ISolrReadOnlyOperations = typeof(ISolrReadOnlyOperations<>).MakeGenericType(core.DocumentType);
            var ISolrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(core.DocumentType);
            var ISolrOperations = typeof(ISolrOperations<>).MakeGenericType(core.DocumentType);
            var SolrServer = typeof (SolrServer<>).MakeGenericType(core.DocumentType);
            var injectionConstructor = new InjectionConstructor(
                new ResolvedParameter(ISolrBasicOperations, core.Id),
                new ResolvedParameter(typeof (IReadOnlyMappingManager)),
                new ResolvedParameter(typeof (IMappingValidator)));
            container.RegisterType(ISolrOperations, SolrServer, core.Id,injectionConstructor);
            container.RegisterType(ISolrReadOnlyOperations, SolrServer, core.Id, injectionConstructor);
        }

        private static void RegisterBasicOperations(SolrCore core, IUnityContainer container) {
            var ISolrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(core.DocumentType);
            var SolrBasicServer = typeof (SolrBasicServer<>).MakeGenericType(core.DocumentType);
            var ISolrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(core.DocumentType);
            var ISolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);

            string coreConnectionId = GetCoreConnectionId(core);
            container.RegisterType(
                ISolrBasicOperations, SolrBasicServer, core.Id,
                new InjectionConstructor(
                    new ResolvedParameter(typeof (ISolrConnection), coreConnectionId),
                    new ResolvedParameter(ISolrQueryExecuter, core.Id),
                    new ResolvedParameter(typeof (ISolrDocumentSerializer<>).MakeGenericType(core.DocumentType)),
                    new ResolvedParameter(typeof (ISolrSchemaParser)),
                    new ResolvedParameter(typeof (ISolrHeaderResponseParser)),
                    new ResolvedParameter(typeof (ISolrQuerySerializer)),
                    new ResolvedParameter(typeof (ISolrDIHStatusParser)),
                    new ResolvedParameter(typeof (ISolrExtractResponseParser))));

            container.RegisterType(
                ISolrBasicReadOnlyOperations, SolrBasicServer, core.Id,
                new InjectionConstructor(
                    new ResolvedParameter(typeof (ISolrConnection), coreConnectionId),
                    new ResolvedParameter(ISolrQueryExecuter, core.Id),
                    new ResolvedParameter(typeof (ISolrDocumentSerializer<>).MakeGenericType(core.DocumentType)),
                    new ResolvedParameter(typeof (ISolrSchemaParser)),
                    new ResolvedParameter(typeof (ISolrHeaderResponseParser)),
                    new ResolvedParameter(typeof (ISolrQuerySerializer)),
                    new ResolvedParameter(typeof (ISolrDIHStatusParser)),
                    new ResolvedParameter(typeof (ISolrExtractResponseParser))));
        }

        private static void RegisterSolrQueryExecuter(SolrCore core, IUnityContainer container) {
            var ISolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            var SolrQueryExecuter = typeof (SolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            string coreConnectionId = GetCoreConnectionId(core);
            container.RegisterType(
                ISolrQueryExecuter, SolrQueryExecuter, core.Id,
                new InjectionConstructor(
                    new ResolvedParameter(typeof (ISolrAbstractResponseParser<>).MakeGenericType(core.DocumentType)),
                    new ResolvedParameter(typeof (ISolrConnection), coreConnectionId),
                    new ResolvedParameter(typeof (ISolrQuerySerializer)),
                    new ResolvedParameter(typeof (ISolrFacetQuerySerializer)),
                    new ResolvedParameter(typeof (ISolrMoreLikeThisHandlerQueryResultsParser<>).MakeGenericType(core.DocumentType))));
        }

        private static string GetCoreConnectionId(SolrCore core) {
            return core.Id + typeof (SolrConnection);
        }

        private void AddCoresFromConfig(SolrServers solrServers, IUnityContainer container) {
            if (solrServers == null) {
                return;
            }

            var cores =
                from server in solrServers.Cast<SolrServerElement>()
                select GetCoreFrom(server);

            foreach (var core in cores) {
                RegisterCore(core, container);
            }
        }

        private static SolrCore GetCoreFrom(SolrServerElement server) {
            var id = server.Id ?? Guid.NewGuid().ToString();
            var documentType = GetCoreDocumentType(server);
            var coreUrl = GetCoreUrl(server);
            UriValidator.ValidateHTTP(coreUrl);
            return new SolrCore(id, documentType, coreUrl);
        }

        private static string GetCoreUrl(SolrServerElement server) {
            var url = server.Url;
            if (string.IsNullOrEmpty(url)) {
                throw new ConfigurationErrorsException("Core url missing in SolrNet core configuration");
            }
            return url;
        }

        private static Type GetCoreDocumentType(SolrServerElement server) {
            var documentType = server.DocumentType;

            if (string.IsNullOrEmpty(documentType)) {
                throw new ConfigurationErrorsException("Document type missing in SolrNet core configuration");
            }

            Type type;

            try {
                type = Type.GetType(documentType);
            } catch (Exception e) {
                throw new ConfigurationErrorsException(string.Format("Error getting document type '{0}'", documentType), e);
            }

            if (type == null)
                throw new ConfigurationErrorsException(string.Format("Error getting document type '{0}'", documentType));

            return type;
        }
    }
}