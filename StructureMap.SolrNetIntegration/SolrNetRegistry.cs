using System;
using System.Collections.Generic;
using System.Configuration;
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
using StructureMap.SolrNetIntegration.Config;

namespace StructureMap.SolrNetIntegration
{
    public class SolrNetRegistry : Configuration.DSL.Registry
    {
        public SolrNetRegistry(SolrServers solrServers)
        {          
            For<IReadOnlyMappingManager>().Use<MemoizingMappingManager>()
                .Ctor<IReadOnlyMappingManager>("mapper").Is(new AttributesMappingManager());
            For(typeof (ISolrDocumentActivator<>)).Use(typeof (SolrDocumentActivator<>));
            For(typeof(ISolrQueryExecuter<>)).Use(typeof(SolrQueryExecuter<>));
            For<ISolrDocumentPropertyVisitor>().Use<DefaultDocumentVisitor>();
            For<IMappingValidator>().Use<MappingValidator>();

            RegisterParsers();
            RegisterValidationRules();
            RegisterSerializers();
            RegisterOperations();

            AddCoresFromConfig(solrServers);
        }

        private void RegisterValidationRules() {
            var validationRules = new[] {
                                            typeof(MappedPropertiesIsInSolrSchemaRule),
                                            typeof(RequiredFieldsAreMappedRule),
                                            typeof(UniqueKeyMatchesMappingRule),
                                        };
            foreach (var validationRule in validationRules)
                For(typeof (IValidationRule)).Use(validationRule);
        }

        private void RegisterSerializers() {
            For(typeof(ISolrDocumentSerializer<>)).Use(typeof(SolrDocumentSerializer<>));
            For(typeof(ISolrDocumentSerializer<Dictionary<string, object>>)).Use(typeof(SolrDictionarySerializer));
            For<ISolrFieldSerializer>().Use<DefaultFieldSerializer>();
            For<ISolrQuerySerializer>().Use<DefaultQuerySerializer>();
            For<ISolrFacetQuerySerializer>().Use<DefaultFacetQuerySerializer>();
        }

        private void RegisterOperations() {
            For(typeof(ISolrBasicReadOnlyOperations<>)).Use(typeof(SolrBasicServer<>));
            For(typeof(ISolrBasicOperations<>)).Use(typeof(SolrBasicServer<>));
            For(typeof(ISolrReadOnlyOperations<>)).Use(typeof(SolrServer<>));
            For(typeof(ISolrOperations<>)).Use(typeof(SolrServer<>));
        }

        private void RegisterParsers() {
            For(typeof(ISolrDocumentResponseParser<>)).Use(typeof(SolrDocumentResponseParser<>));

            For<ISolrDocumentResponseParser<Dictionary<string, object>>>()
                .Use<SolrDictionaryDocumentResponseParser>();

            var parsers = new[] {
                                    typeof(ResultsResponseParser<>),
                                    typeof(HeaderResponseParser<>),
                                    typeof(FacetsResponseParser<>),
                                    typeof(HighlightingResponseParser<>),
                                    typeof(MoreLikeThisResponseParser<>),
                                    typeof(SpellCheckResponseParser<>),
                                    typeof(StatsResponseParser<>),
                                    typeof(CollapseResponseParser<>),
                                    typeof(GroupingResponseParser<>),
                                    typeof(ClusterResponseParser<>),
                                    typeof(TermsResponseParser<>)
                                };

            foreach (var p in parsers)
            {
                For(typeof(ISolrResponseParser<>)).Use(p);
            }

            For<ISolrHeaderResponseParser>().Use<HeaderResponseParser<string>>();
            For<ISolrExtractResponseParser>().Use<ExtractResponseParser>();
            For(typeof(ISolrQueryResultParser<>)).Use(typeof(SolrQueryResultParser<>));
            For<ISolrFieldParser>().Use<DefaultFieldParser>();
            For<ISolrSchemaParser>().Use<SolrSchemaParser>();
            For<ISolrDIHStatusParser>().Use<SolrDIHStatusParser>();
        }

        /// <summary>
        /// Registers a new core in the container.
        /// This method is meant to be used after the facility initialization
        /// </summary>
        /// <param name="core"></param>
        private void RegisterCore(SolrCore core) {
            var coreConnectionId = core.Id + typeof(SolrConnection);

            For<ISolrConnection>().Add<SolrConnection>().Named(coreConnectionId).Ctor<string>("serverURL").Is(core.Url);

            var ISolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            var SolrQueryExecuter = typeof(SolrQueryExecuter<>).MakeGenericType(core.DocumentType);

            For(ISolrQueryExecuter).Add(SolrQueryExecuter).Named(core.Id + SolrQueryExecuter)
                .CtorDependency<ISolrConnection>("connection").IsNamedInstance(coreConnectionId);

            var ISolrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(core.DocumentType);
            var ISolrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(core.DocumentType);
            var SolrBasicServer = typeof(SolrBasicServer<>).MakeGenericType(core.DocumentType);

            For(ISolrBasicOperations).Add(SolrBasicServer).Named(core.Id + SolrBasicServer)
                .CtorDependency<ISolrConnection>("connection").IsNamedInstance(coreConnectionId)
                .Child("queryExecuter").IsNamedInstance(core.Id + SolrQueryExecuter);

            For(ISolrBasicReadOnlyOperations).Add(SolrBasicServer).Named(core.Id + SolrBasicServer)
                .CtorDependency<ISolrConnection>("connection").IsNamedInstance(coreConnectionId)
                .Child("queryExecuter").IsNamedInstance(core.Id + SolrQueryExecuter);

            var ISolrOperations = typeof(ISolrOperations<>).MakeGenericType(core.DocumentType);
            var SolrServer = typeof(SolrServer<>).MakeGenericType(core.DocumentType);
            For(ISolrOperations).Add(SolrServer).Named(core.Id)
                .Child("basicServer").IsNamedInstance(core.Id + SolrBasicServer);
        }

        private void AddCoresFromConfig(SolrServers solrServers)
        {
            if (solrServers == null)
                return;

            var cores = new List<SolrCore>();

            foreach (SolrServerElement server in solrServers) {
                var solrCore = GetCoreFrom(server);
                cores.Add(solrCore);
            }

            foreach (var core in cores) {
                RegisterCore(core);
            }
        }

        private static SolrCore GetCoreFrom(SolrServerElement server)
        {
            var id = server.Id ?? Guid.NewGuid().ToString();
            var documentType = GetCoreDocumentType(server);
            var coreUrl = GetCoreUrl(server);
            ValidateUrl(coreUrl);
            return new SolrCore(id, documentType, coreUrl);
        }

        private static string GetCoreUrl(SolrServerElement server) 
        {
            var url = server.Url;
            if (string.IsNullOrEmpty(url))
                throw new ConfigurationErrorsException("Core url missing in SolrNet core configuration");
            return url;
        }

        private static Type GetCoreDocumentType(SolrServerElement server) 
        {
            var documentType = server.DocumentType;
            
            if (string.IsNullOrEmpty(documentType))
                throw new ConfigurationErrorsException("Document type missing in SolrNet core configuration");
            
            Type type;
            
            try 
            {
                type = Type.GetType(documentType);
            }
            catch (Exception e) 
            {
                throw new ConfigurationErrorsException(string.Format("Error getting document type '{0}'", documentType), e);
            }
            
            if (type == null)
                throw new ConfigurationErrorsException(string.Format("Error getting document type '{0}'", documentType));
            
            return type;
        }

        private static void ValidateUrl(string url)
        {
            try
            {
                var u = new Uri(url);
                if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
                    throw new InvalidURLException("Only HTTP or HTTPS protocols are supported");
            }
            catch (ArgumentException e)
            {
                throw new InvalidURLException(string.Format("Invalid URL '{0}'", url), e);
            }
            catch (UriFormatException e)
            {
                throw new InvalidURLException(string.Format("Invalid URL '{0}'", url), e);
            }
        }
    }
}