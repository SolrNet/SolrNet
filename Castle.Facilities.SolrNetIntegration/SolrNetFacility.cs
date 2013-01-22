#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
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

namespace Castle.Facilities.SolrNetIntegration {
    /// <summary>
    /// Configures SolrNet in a Windsor container
    /// </summary>
    public class SolrNetFacility : AbstractFacility {
        private readonly string solrURL;

        /// <summary>
        /// Default mapper override
        /// </summary>
        public IReadOnlyMappingManager Mapper { get; set; }

        /// <summary>
        /// Configures SolrNet in a Windsor container
        /// </summary>
        public SolrNetFacility() {}

        /// <summary>
        /// Configures SolrNet in a Windsor container
        /// </summary>
        /// <param name="solrURL"></param>
        public SolrNetFacility(string solrURL) {
            ValidateUrl(solrURL);
            this.solrURL = solrURL;
        }

        private string GetSolrUrl() {
            if (solrURL != null)
                return solrURL;
            if (FacilityConfig == null)
                throw new FacilityException("Please add solrURL to the SolrNetFacility configuration");
            var configNode = FacilityConfig.Children["solrURL"];
            if (configNode == null)
                throw new FacilityException("Please add solrURL to the SolrNetFacility configuration");
            var url = configNode.Value;
            ValidateUrl(url);
            return url;
        }

        protected override void Init() {
            var mapper = Mapper ?? new MemoizingMappingManager(new AttributesMappingManager());
            Kernel.Register(Component.For<IReadOnlyMappingManager>().Instance(mapper));
            //Kernel.Register(Component.For<ISolrCache>().ImplementedBy<HttpRuntimeCache>());
            Kernel.Register(Component.For<ISolrConnection>().ImplementedBy<SolrConnection>()
                                .Parameters(Parameter.ForKey("serverURL").Eq(GetSolrUrl())));

            Kernel.Register(Component.For(typeof (ISolrDocumentActivator<>)).ImplementedBy(typeof(SolrDocumentActivator<>)));

            Kernel.Register(Component.For(typeof (ISolrDocumentResponseParser<>))
                .ImplementedBy(typeof (SolrDocumentResponseParser<>)));
            Kernel.Register(Component.For<ISolrDocumentResponseParser<Dictionary<string, object>>>()
                .ImplementedBy<SolrDictionaryDocumentResponseParser>());

            Kernel.Register(Component.For(typeof (ISolrAbstractResponseParser<>)).ImplementedBy(typeof (DefaultResponseParser<>)));

            Kernel.Register(Component.For<ISolrHeaderResponseParser>().ImplementedBy<HeaderResponseParser<string>>());
            Kernel.Register(Component.For<ISolrExtractResponseParser>().ImplementedBy<ExtractResponseParser>());
            foreach (var validationRule in new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
                typeof(MultivaluedMappedToCollectionRule),
            })
                Kernel.Register(Component.For<IValidationRule>().ImplementedBy(validationRule));
            Kernel.Resolver.AddSubResolver(new StrictArrayResolver(Kernel));
            Kernel.Register(Component.For(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>))
                .ImplementedBy(typeof(SolrMoreLikeThisHandlerQueryResultsParser<>)));
            Kernel.Register(Component.For(typeof (ISolrQueryExecuter<>)).ImplementedBy(typeof (SolrQueryExecuter<>)));

            Kernel.Register(Component.For(typeof (ISolrDocumentSerializer<>))
                .ImplementedBy(typeof (SolrDocumentSerializer<>)));
            Kernel.Register(Component.For<ISolrDocumentSerializer<Dictionary<string, object>>>()
                .ImplementedBy<SolrDictionarySerializer>());

            Kernel.Register(Component.For(typeof (ISolrBasicOperations<>), typeof (ISolrBasicReadOnlyOperations<>))
                                .ImplementedBy(typeof (SolrBasicServer<>)));
            Kernel.Register(Component.For(typeof (ISolrOperations<>), typeof (ISolrReadOnlyOperations<>))
                                .ImplementedBy(typeof (SolrServer<>)));

            Kernel.Register(Component.For<ISolrFieldParser>()
                .ImplementedBy<DefaultFieldParser>());

            Kernel.Register(Component.For<ISolrFieldSerializer>().ImplementedBy<DefaultFieldSerializer>());

            Kernel.Register(Component.For<ISolrQuerySerializer>().ImplementedBy<DefaultQuerySerializer>());
            Kernel.Register(Component.For<ISolrFacetQuerySerializer>().ImplementedBy<DefaultFacetQuerySerializer>());

            Kernel.Register(Component.For<ISolrDocumentPropertyVisitor>().ImplementedBy<DefaultDocumentVisitor>());

            Kernel.Register(Component.For<ISolrSchemaParser>().ImplementedBy<SolrSchemaParser>());
            Kernel.Register(Component.For<ISolrDIHStatusParser>().ImplementedBy<SolrDIHStatusParser>());
            Kernel.Register(Component.For<IMappingValidator>().ImplementedBy<MappingValidator>());

            Kernel.Register(Component.For<ISolrStatusResponseParser>().ImplementedBy<SolrStatusResponseParser>());
            Kernel.Register(Component.For<ISolrCoreAdmin>().ImplementedBy<SolrCoreAdmin>());

            AddCoresFromConfig();
            foreach (var core in cores) {
                RegisterCore(core);
            }
        }

        /// <summary>
        /// Registers a new core in the container.
        /// This method is meant to be used after the facility initialization
        /// </summary>
        /// <param name="core"></param>
        private void RegisterCore(SolrCore core) {
            var coreConnectionId = core.Id + typeof (SolrConnection);
            Kernel.Register(Component.For<ISolrConnection>().ImplementedBy<SolrConnection>()
                                .Named(coreConnectionId)
                                .Parameters(Parameter.ForKey("serverURL").Eq(core.Url)));

            var ISolrQueryExecuter = typeof (ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            var SolrQueryExecuter = typeof (SolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            Kernel.Register(Component.For(ISolrQueryExecuter).ImplementedBy(SolrQueryExecuter)
                                .Named(core.Id + SolrQueryExecuter)
                                .ServiceOverrides(ServiceOverride.ForKey("connection").Eq(coreConnectionId)));

            var ISolrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(core.DocumentType);
            var ISolrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(core.DocumentType);
            var SolrBasicServer = typeof(SolrBasicServer<>).MakeGenericType(core.DocumentType);
            Kernel.Register(Component.For(ISolrBasicOperations, ISolrBasicReadOnlyOperations)
                                .ImplementedBy(SolrBasicServer)
                                .Named(core.Id + SolrBasicServer)
                                .ServiceOverrides(ServiceOverride.ForKey("connection").Eq(coreConnectionId),
                                    ServiceOverride.ForKey("queryExecuter").Eq(core.Id + SolrQueryExecuter)));

            var ISolrOperations = typeof (ISolrOperations<>).MakeGenericType(core.DocumentType);
            var SolrServer = typeof(SolrServer<>).MakeGenericType(core.DocumentType);
            Kernel.Register(Component.For(ISolrOperations).ImplementedBy(SolrServer)
                                .Named(core.Id)
                                .ServiceOverrides(ServiceOverride.ForKey("basicServer").Eq(core.Id + SolrBasicServer)));
        }

        /// <summary>
        /// Adds a new core configuration to the facility
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="coreUrl"></param>
        public void AddCore(Type documentType, string coreUrl) {
            AddCore(Guid.NewGuid().ToString(), documentType, coreUrl);
        }

        /// <summary>
        /// Adds a new core configuration to the facility
        /// </summary>
        /// <param name="coreId">Component name for <see cref="ISolrOperations{T}"/></param>
        /// <param name="documentType"></param>
        /// <param name="coreUrl"></param>
        public void AddCore(string coreId, Type documentType, string coreUrl) {
            ValidateUrl(coreUrl);
            cores.Add(new SolrCore(coreId, documentType, coreUrl));
        }

        private void AddCoresFromConfig() {
            if (FacilityConfig == null)
                return;
            var coresConfig = FacilityConfig.Children["cores"];
            if (coresConfig == null)
                return;
            foreach (var coreConfig in coresConfig.Children) {
                var id = coreConfig.Attributes["id"] ?? Guid.NewGuid().ToString();
                var documentType = GetCoreDocumentType(coreConfig);
                var coreUrl = GetCoreUrl(coreConfig);
                AddCore(id, documentType, coreUrl);
            }
        }

        private string GetCoreUrl(IConfiguration coreConfig) {
            var node = coreConfig.Children["url"];
            if (node == null)
                throw new FacilityException("Core url missing in SolrNet core configuration");
            return node.Value;
        }

        private Type GetCoreDocumentType(IConfiguration coreConfig) {
            var node = coreConfig.Children["documentType"];
            if (node == null)
                throw new FacilityException("Document type missing in SolrNet core configuration");
            try {
                return Type.GetType(node.Value);                
            } catch (Exception e) {
                throw new FacilityException(string.Format("Error getting document type '{0}'", node.Value), e);
            }
        }

        /// <summary>
        /// Builds an instance of core admin manager with the specified URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ISolrCoreAdmin BuildCoreAdmin(string url) {
            var conn = new SolrConnection(url);
            return BuildCoreAdmin(conn);
        }

        /// <summary>
        /// Builds an instance of core admin manager with the specified connection
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public ISolrCoreAdmin BuildCoreAdmin(ISolrConnection conn) {
            return new SolrCoreAdmin(conn, Kernel.Resolve<ISolrHeaderResponseParser>(), Kernel.Resolve<ISolrStatusResponseParser>());            
        }

        private static void ValidateUrl(string s) {
            try {
                UriValidator.ValidateHTTP(s);
            } catch (InvalidURLException e) {
                throw new FacilityException("", e);
            }
        }

        private readonly List<SolrCore> cores = new List<SolrCore>();
    }
}