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
using System.Configuration;
using Ninject.Integration.SolrNet.Config;
using Ninject.Modules;
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

namespace Ninject.Integration.SolrNet {
    /// <summary>
    /// Configures SolrNet in a Ninject kernel
    /// </summary>
    public class SolrNetModule : NinjectModule {
        private readonly string serverURL;
        private readonly List<SolrCore> cores = new List<SolrCore>();
        private const string CoreId = "CoreId";

        /// <summary>
        /// Optional override for document mapper
        /// </summary>
        public IReadOnlyMappingManager Mapper { get; set; }

        /// <summary>
        /// Configures SolrNet in a Ninject kernel
        /// </summary>
        /// <param name="serverURL"></param>
        public SolrNetModule(string serverURL) {
            this.serverURL = serverURL;
        }

        /// <summary>
        /// Configures SolrNet in a Ninject kernel with multiple servers/cores
        /// </summary>
        /// <param name="solrServers"></param>
        public SolrNetModule(SolrServers solrServers) {
            AddCoresFromConfig(solrServers);            
        }

        private void AddCoresFromConfig(SolrServers solrServers) {
            if (solrServers == null) {
                return;
            }

            foreach (SolrServerElement server in solrServers)
            {
                var solrCore = GetCoreFrom(server);
                cores.Add(solrCore);
            }

        }

        private static SolrCore GetCoreFrom(SolrServerElement server) {
            var id = server.Id ?? Guid.NewGuid().ToString();
            var documentType = GetCoreDocumentType(server);
            var coreUrl = GetCoreUrl(server);
            UriValidator.ValidateHTTP(coreUrl);
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

        private void RegisterCore(SolrCore core) {
            string coreConnectionId = core.Id;

            Bind<ISolrConnection>().ToConstant(new SolrConnection(core.Url))
                .WithMetadata(CoreId, coreConnectionId);

            var iSolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            var solrQueryExecuter = typeof(SolrQueryExecuter<>).MakeGenericType(core.DocumentType);

            Bind(iSolrQueryExecuter).To(solrQueryExecuter)
                .Named(coreConnectionId + solrQueryExecuter)
                .WithMetadata(CoreId, coreConnectionId)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CoreId) && bindingMetaData.Get<string>(CoreId).Equals(coreConnectionId)));

            var solrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(core.DocumentType);
            var solrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(core.DocumentType);
            var solrBasicServer = typeof(SolrBasicServer<>).MakeGenericType(core.DocumentType);

            Bind(solrBasicOperations).To(solrBasicServer)
                .Named(coreConnectionId + solrBasicServer)
                .WithMetadata(CoreId, coreConnectionId)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CoreId) && bindingMetaData.Get<string>(CoreId).Equals(coreConnectionId)))
                .WithConstructorArgument("queryExecuter", ctx => ctx.Kernel.Get(iSolrQueryExecuter, bindingMetaData => bindingMetaData.Has(CoreId) && bindingMetaData.Get<string>(CoreId).Equals(coreConnectionId)));

            Bind(solrBasicReadOnlyOperations).To(solrBasicServer)
                .Named(coreConnectionId + solrBasicServer)
                .WithMetadata(CoreId, coreConnectionId)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CoreId) && bindingMetaData.Get<string>(CoreId).Equals(coreConnectionId)))
                .WithConstructorArgument("queryExecuter", ctx => ctx.Kernel.Get(iSolrQueryExecuter, bindingMetaData => bindingMetaData.Has(CoreId) && bindingMetaData.Get<string>(CoreId).Equals(coreConnectionId)));

            var solrOperations = typeof(ISolrOperations<>).MakeGenericType(core.DocumentType);
            var solrServer = typeof(SolrServer<>).MakeGenericType(core.DocumentType);
            var solrReadOnlyOperations = typeof(ISolrReadOnlyOperations<>).MakeGenericType(core.DocumentType);

            Bind(solrOperations).To(solrServer)
                .Named(core.Id)
                .WithMetadata(CoreId, coreConnectionId)
                .WithConstructorArgument("basicServer", ctx => ctx.Kernel.Get(solrBasicOperations, bindingMetaData => bindingMetaData.Has(CoreId) && bindingMetaData.Get<string>(CoreId).Equals(coreConnectionId)));
            Bind(solrReadOnlyOperations).To(solrServer)
                .Named(core.Id)
                .WithMetadata(CoreId, coreConnectionId)
                .WithConstructorArgument("basicServer", ctx => ctx.Kernel.Get(solrBasicReadOnlyOperations, bindingMetaData => bindingMetaData.Has(CoreId) && bindingMetaData.Get<string>(CoreId).Equals(coreConnectionId)));
        }

        public override void Load() {
            var mapper = Mapper ?? new MemoizingMappingManager(new AttributesMappingManager());
            Bind<IReadOnlyMappingManager>().ToConstant(mapper);
            //Bind<ISolrCache>().To<HttpRuntimeCache>();
            Bind<ISolrDocumentPropertyVisitor>().To<DefaultDocumentVisitor>();
            Bind<ISolrFieldParser>().To<DefaultFieldParser>();
            Bind(typeof (ISolrDocumentActivator<>)).To(typeof (SolrDocumentActivator<>));
            Bind(typeof(ISolrDocumentResponseParser<>)).To(typeof(SolrDocumentResponseParser<>));
            Bind<ISolrDocumentResponseParser<Dictionary<string, object>>>().To<SolrDictionaryDocumentResponseParser>();
            Bind<ISolrFieldSerializer>().To<DefaultFieldSerializer>();
            Bind<ISolrQuerySerializer>().To<DefaultQuerySerializer>();
            Bind<ISolrFacetQuerySerializer>().To<DefaultFacetQuerySerializer>();
            Bind(typeof (ISolrAbstractResponseParser<>)).To(typeof (DefaultResponseParser<>));
            Bind<ISolrHeaderResponseParser>().To<HeaderResponseParser<string>>();
            Bind<ISolrExtractResponseParser>().To<ExtractResponseParser>();
            foreach (var p in new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
                typeof(MultivaluedMappedToCollectionRule),
            })
                Bind<IValidationRule>().To(p);
            Bind(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>)).To(typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
            Bind(typeof(ISolrDocumentSerializer<>)).To(typeof(SolrDocumentSerializer<>));
            Bind(typeof(ISolrDocumentSerializer<Dictionary<string, object>>)).To(typeof(SolrDictionarySerializer));

            Bind<ISolrSchemaParser>().To<SolrSchemaParser>();
            Bind<ISolrDIHStatusParser>().To<SolrDIHStatusParser>();
            Bind<IMappingValidator>().To<MappingValidator>();

            if (cores.Count != 0)
            {
                Bind<ISolrStatusResponseParser>().To<SolrStatusResponseParser>();
                Bind<ISolrCoreAdmin>().To<SolrCoreAdmin>();

                foreach (var solrCore in cores)
                {
                    RegisterCore(solrCore);
                }
            }
            else {
                //Bind single type to a single url, prevent breaking existing functionality
                Bind<ISolrConnection>().ToConstant(new SolrConnection(serverURL));
                Bind(typeof (ISolrQueryExecuter<>)).To(typeof (SolrQueryExecuter<>));
                Bind(typeof (ISolrBasicOperations<>)).To(typeof (SolrBasicServer<>));
                Bind(typeof (ISolrBasicReadOnlyOperations<>)).To(typeof (SolrBasicServer<>));
                Bind(typeof (ISolrOperations<>)).To(typeof (SolrServer<>));
                Bind(typeof (ISolrReadOnlyOperations<>)).To(typeof (SolrServer<>));
            }
        }
    }
}