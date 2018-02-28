using System;
using System.Collections.Generic;
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
using SolrNet.Utils;
using StructureMap.Pipeline;

namespace StructureMap.SolrNetIntegration
{
    public class SolrNetRegistry : Registry
    {
        // Constructor should not be used to create SolrNetRegistry as it's 
        private SolrNetRegistry()
        {
        }

        public static SolrNetRegistry Create(IEnumerable<ISolrServer> solrServers)
        {
            var registry = new SolrNetRegistry();
            registry.Initialize(solrServers);
            return registry;
        }

        protected virtual void Initialize(IEnumerable<ISolrServer> solrServers)
        {
            For<IReadOnlyMappingManager>().Use<MemoizingMappingManager>()
                .Ctor<IReadOnlyMappingManager>("mapper").Is(new AttributesMappingManager());
            For(typeof(ISolrDocumentActivator<>)).Use(typeof(SolrDocumentActivator<>));
            For(typeof(ISolrQueryExecuter<>)).Use(typeof(SolrQueryExecuter<>));
            For<ISolrDocumentPropertyVisitor>().Use<DefaultDocumentVisitor>();
            For<IMappingValidator>().Use<MappingValidator>();
            For<ISolrCache>().Use<NullCache>();

            RegisterParsers();
            RegisterValidationRules();
            RegisterSerializers();
            RegisterOperations();
            RegisterServers(solrServers);
        }

        protected virtual void RegisterValidationRules()
        {
            var validationRules = new[] {
                                            typeof(MappedPropertiesIsInSolrSchemaRule),
                                            typeof(RequiredFieldsAreMappedRule),
                                            typeof(UniqueKeyMatchesMappingRule),
                                            typeof(MultivaluedMappedToCollectionRule),
                                        };
            foreach (var validationRule in validationRules)
                For(typeof(IValidationRule)).Use(validationRule);
        }

        protected virtual void RegisterSerializers()
        {
            For(typeof(ISolrDocumentSerializer<>)).Use(typeof(SolrDocumentSerializer<>));
            For(typeof(ISolrDocumentSerializer<Dictionary<string, object>>)).Use(typeof(SolrDictionarySerializer));
            For<ISolrFieldSerializer>().Use<DefaultFieldSerializer>();
            For<ISolrQuerySerializer>().Use<DefaultQuerySerializer>();
            For<ISolrFacetQuerySerializer>().Use<DefaultFacetQuerySerializer>();
        }

        public virtual void RegisterOperations()
        {
            For(typeof(ISolrBasicReadOnlyOperations<>)).Use(typeof(SolrBasicServer<>));
            For(typeof(ISolrBasicOperations<>)).Use(typeof(SolrBasicServer<>));
            For(typeof(ISolrReadOnlyOperations<>)).Use(typeof(SolrServer<>));
            For(typeof(ISolrOperations<>)).Use(typeof(SolrServer<>));
        }

        protected virtual void RegisterParsers()
        {
            For(typeof(ISolrDocumentResponseParser<>)).Use(typeof(SolrDocumentResponseParser<>));

            For<ISolrDocumentResponseParser<Dictionary<string, object>>>()
                .Use<SolrDictionaryDocumentResponseParser>();

            For(typeof(ISolrAbstractResponseParser<>)).Use(typeof(DefaultResponseParser<>));

            For<ISolrHeaderResponseParser>().Use<HeaderResponseParser<string>>();
            For<ISolrExtractResponseParser>().Use<ExtractResponseParser>();
            For(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>)).Use(typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
            For<ISolrFieldParser>().Use<DefaultFieldParser>();
            For<ISolrSchemaParser>().Use<SolrSchemaParser>();
            For<ISolrDIHStatusParser>().Use<SolrDIHStatusParser>();
            For<ISolrStatusResponseParser>().Use<SolrStatusResponseParser>();
            For<ISolrCoreAdmin>().Use<SolrCoreAdmin>();
        }

        /// <summary>
        /// Registers a new core in the container.
        /// This method is meant to be used after the facility initialization
        /// </summary>
        protected virtual void RegisterCore(string id, Type documentType, string coreUrl)
        {
            var coreConnectionId = id + typeof(SolrConnection);

            For<ISolrConnection>().Use(() => new AutoSolrConnection(coreUrl))
                .Named(coreConnectionId);
            
            var ISolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(documentType);
            var SolrQueryExecuter = typeof(SolrQueryExecuter<>).MakeGenericType(documentType);

            For(ISolrQueryExecuter).Add(SolrQueryExecuter).Named(id + SolrQueryExecuter)
                .Ctor<ISolrConnection>("connection").IsNamedInstance(coreConnectionId);

            var ISolrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(documentType);
            var ISolrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(documentType);
            var SolrBasicServer = typeof(SolrBasicServer<>).MakeGenericType(documentType);

            For(ISolrBasicOperations).Add(SolrBasicServer).Named(id + SolrBasicServer)
                .Ctor<ISolrConnection>("connection").IsNamedInstance(coreConnectionId)
                .Dependencies.Add("queryExecuter", new ReferencedInstance(id + SolrQueryExecuter));


            For(ISolrBasicReadOnlyOperations).Add(SolrBasicServer).Named(id + SolrBasicServer)
                .Ctor<ISolrConnection>("connection").IsNamedInstance(coreConnectionId)
                .Dependencies.Add("queryExecuter", new ReferencedInstance(id + SolrQueryExecuter));


            var ISolrOperations = typeof(ISolrOperations<>).MakeGenericType(documentType);
            var SolrServer = typeof(SolrServer<>).MakeGenericType(documentType);
            For(ISolrOperations).Add(SolrServer).Named(id)
                 .Dependencies.Add("basicServer", new ReferencedInstance(id + SolrBasicServer));
        }

        protected virtual void RegisterServers(IEnumerable<ISolrServer> servers)
        {            
            foreach (var server in servers)
            {
                RegisterCore(server.Id ?? Guid.NewGuid().ToString(), GetCoreDocumentType(server), GetCoreUrl(server));
            }
        }

        protected virtual string GetCoreUrl(ISolrServer server)
        {
            var url = server.Url;
            if (string.IsNullOrEmpty(url))
                throw new StructureMapConfigurationException("Core url missing in SolrNet core configuration");
            
            UriValidator.ValidateHTTP(url);
            return url;
        }

        protected virtual Type GetCoreDocumentType(ISolrServer server)
        {
            var documentType = server.DocumentType;

            if (string.IsNullOrEmpty(documentType))
                throw new StructureMapConfigurationException("Document type missing in SolrNet core configuration");

            Type type;

            try
            {
                type = Type.GetType(documentType);
            }
            catch (Exception e)
            {
                throw new StructureMapConfigurationException($"Error getting document type '{documentType}'", e);
            }

            if (type == null)
                throw new StructureMapConfigurationException($"Error getting document type '{documentType}'");

            return type;
        }

    }
}
