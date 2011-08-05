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
using Autofac;
using Autofac.Core;
using AutofacContrib.SolrNet.Config;
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

namespace AutofacContrib.SolrNet
{
    public class SolrNetModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            if (!string.IsNullOrEmpty(ServerUrl))
                RegisterSingleCore(builder);
            else if (_solrServers != null)
                RegisterMultiCore(builder);
            else
                throw new ConfigurationErrorsException("SolrNetModule Configurations Error!");
        }

        #region SingleCore Registration

        /// <summary>
        /// Register a single-core server
        /// </summary>
        /// <param name="serverUrl"></param>
        public SolrNetModule(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        private readonly string ServerUrl;
        public IReadOnlyMappingManager Mapper { get; set; }

        private void RegisterSingleCore(ContainerBuilder builder)
        {
            var mapper = Mapper ?? new MemoizingMappingManager(new AttributesMappingManager());
            builder.RegisterInstance(mapper).As<IReadOnlyMappingManager>();
            //builder.RegisterType<HttpRuntimeCache>().As<ISolrCache>();
            builder.RegisterType<DefaultDocumentVisitor>().As<ISolrDocumentPropertyVisitor>();
            builder.RegisterType<DefaultFieldParser>().As<ISolrFieldParser>();
            builder.RegisterGeneric(typeof(SolrDocumentActivator<>)).As(typeof(ISolrDocumentActivator<>));
            builder.RegisterGeneric(typeof(SolrDocumentResponseParser<>)).As(typeof(ISolrDocumentResponseParser<>));
            builder.RegisterType<DefaultFieldSerializer>().As<ISolrFieldSerializer>();
            builder.RegisterType<DefaultQuerySerializer>().As<ISolrQuerySerializer>();
            builder.RegisterType<DefaultFacetQuerySerializer>().As<ISolrFacetQuerySerializer>();
            foreach (var p in new[] {
                typeof(ResultsResponseParser<>),
                typeof(HeaderResponseParser<>),
                typeof(FacetsResponseParser<>),
                typeof(HighlightingResponseParser<>),
                typeof(MoreLikeThisResponseParser<>),
                typeof(SpellCheckResponseParser<>),
                typeof(StatsResponseParser<>),
                typeof(CollapseResponseParser<>),
				typeof(GroupingResponseParser<>),
                typeof(ClusterResponseParser<>)
            })
                builder.RegisterGeneric(p).As(typeof(ISolrResponseParser<>));

            builder.RegisterType<HeaderResponseParser<string>>().As<ISolrHeaderResponseParser>();
            builder.RegisterType<ExtractResponseParser>().As<ISolrExtractResponseParser>();
            foreach (var p in new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
            })
                builder.RegisterType(p).As<IValidationRule>();
            builder.RegisterInstance(new SolrConnection(ServerUrl)).As<ISolrConnection>();
            builder.RegisterGeneric(typeof(SolrQueryResultParser<>)).As(typeof(ISolrQueryResultParser<>));
            builder.RegisterGeneric(typeof(SolrQueryExecuter<>)).As(typeof(ISolrQueryExecuter<>));
            builder.RegisterGeneric(typeof(SolrDocumentSerializer<>)).As(typeof(ISolrDocumentSerializer<>));
            builder.RegisterGeneric(typeof(SolrBasicServer<>))
                .As(typeof(ISolrBasicOperations<>), typeof(ISolrBasicReadOnlyOperations<>))
                .SingleInstance();
            builder.RegisterGeneric(typeof(SolrServer<>))
                .As(typeof(ISolrOperations<>), typeof(ISolrReadOnlyOperations<>))
                .SingleInstance();
            builder.RegisterType<SolrSchemaParser>().As<ISolrSchemaParser>();
            builder.RegisterType<SolrDIHStatusParser>().As<ISolrDIHStatusParser>();
            builder.RegisterType<MappingValidator>().As<IMappingValidator>();
        }

        #endregion

        #region Multicore Registration

        private SolrServers _solrServers;

        /// <summary>
        /// Register multi-core server
        /// </summary>
        /// <param name="solrServers"></param>
        public SolrNetModule(SolrServers solrServers)
        {
            _solrServers = solrServers;
        }

        private void RegisterMultiCore(ContainerBuilder builder)
        {
            RegisterInterfaces(builder);
            AddCoresFromConfig(builder);
        }

        private void RegisterInterfaces(ContainerBuilder builder)
        {
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            builder.RegisterInstance(mapper).As<IReadOnlyMappingManager>();
            // builder.RegisterType<HttpRuntimeCache>().As<ISolrCache>();
            builder.RegisterType<DefaultDocumentVisitor>().As<ISolrDocumentPropertyVisitor>();
            builder.RegisterType<DefaultFieldParser>().As<ISolrFieldParser>();
            builder.RegisterGeneric(typeof(SolrDocumentActivator<>)).As(typeof(ISolrDocumentActivator<>));
            builder.RegisterGeneric(typeof(SolrDocumentResponseParser<>)).As(typeof(ISolrDocumentResponseParser<>));
            builder.RegisterType<DefaultFieldSerializer>().As<ISolrFieldSerializer>();
            builder.RegisterType<DefaultQuerySerializer>().As<ISolrQuerySerializer>();
            builder.RegisterType<DefaultFacetQuerySerializer>().As<ISolrFacetQuerySerializer>();
            foreach (var p in new[] {
                typeof(ResultsResponseParser<>),
                typeof(HeaderResponseParser<>),
                typeof(FacetsResponseParser<>),
                typeof(HighlightingResponseParser<>),
                typeof(MoreLikeThisResponseParser<>),
                typeof(SpellCheckResponseParser<>),
                typeof(StatsResponseParser<>),
                typeof(CollapseResponseParser<>),
				typeof(GroupingResponseParser<>),
                typeof(ClusterResponseParser<>)
            })

                builder.RegisterGeneric(p).As(typeof(ISolrResponseParser<>));
            builder.RegisterType<HeaderResponseParser<string>>().As<ISolrHeaderResponseParser>();
            builder.RegisterType<ExtractResponseParser>().As<ISolrExtractResponseParser>();
            foreach (var p in new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
            })

                builder.RegisterType(p).As<IValidationRule>();
            builder.RegisterType<SolrSchemaParser>().As<ISolrSchemaParser>();
            builder.RegisterGeneric(typeof(SolrQueryResultParser<>)).As(typeof(ISolrQueryResultParser<>));
            builder.RegisterGeneric(typeof(SolrDocumentSerializer<>)).As(typeof(ISolrDocumentSerializer<>));
            builder.RegisterType<SolrDIHStatusParser>().As<ISolrDIHStatusParser>();
            builder.RegisterType<MappingValidator>().As<IMappingValidator>();
        }

        /// <summary>
        /// Registers a new core in the container.
        /// This method is meant to be used after the facility initialization
        /// </summary>
        /// <param name="core"></param>
        private void RegisterCore(SolrCore core, ContainerBuilder builder)
        {
            var coreConnectionId = core.Id + typeof(SolrConnection);

            builder.RegisterType(typeof(SolrConnection))
                    .Named(coreConnectionId, typeof(ISolrConnection))
                    .WithParameters(new[] {
                        new NamedParameter("serverURL", core.Url) });

            var ISolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            var SolrQueryExecuter = typeof(SolrQueryExecuter<>).MakeGenericType(core.DocumentType);

            builder.RegisterType(SolrQueryExecuter)
                .Named(core.Id + SolrQueryExecuter, ISolrQueryExecuter)
                .WithParameters(new[] {
                        new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.Resolve(coreConnectionId, typeof(ISolrConnection))),
                        
                    }
                );

            var ISolrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(core.DocumentType);
            var ISolrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(core.DocumentType);
            var SolrBasicServer = typeof(SolrBasicServer<>).MakeGenericType(core.DocumentType);

            builder.RegisterType(SolrBasicServer)
                .Named(core.Id + SolrBasicServer, ISolrBasicOperations)
                .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.Resolve(coreConnectionId, typeof(ISolrConnection))),
                    new ResolvedParameter((p, c) => p.Name == "queryExecuter", (p, c) => c.Resolve(core.Id + SolrQueryExecuter, ISolrQueryExecuter))
                   }
                );

            builder.RegisterType(SolrBasicServer)
                .Named(core.Id + SolrBasicServer, ISolrBasicReadOnlyOperations)
                .WithParameters(new[] {
                                new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.Resolve(coreConnectionId, typeof(ISolrConnection) )),
                                new ResolvedParameter((p, c) => p.Name == "queryExecuter", (p, c) => c.Resolve(core.Id + SolrQueryExecuter, ISolrQueryExecuter))
                               }
                );

            var ISolrOperations = typeof(ISolrOperations<>).MakeGenericType(core.DocumentType);
            var ISolrReadOnlyOperations = typeof(ISolrReadOnlyOperations<>).MakeGenericType(core.DocumentType);
            var SolrServer = typeof(SolrServer<>).MakeGenericType(core.DocumentType);

            builder.RegisterType(SolrServer)
               .As(ISolrOperations)
               .WithParameters(new[] {
                                new ResolvedParameter((p, c) => p.Name == "basicServer", (p, c) => c.Resolve(core.Id + SolrBasicServer, ISolrBasicOperations)),
                               }
               );

            builder.RegisterType(SolrServer)
               .As(ISolrReadOnlyOperations)
               .WithParameters(new[] {
                                new ResolvedParameter((p, c) => p.Name == "basicServer", (p, c) => c.Resolve(core.Id + SolrBasicServer, ISolrBasicOperations)),
                               }
               );
        }

        private void AddCoresFromConfig(ContainerBuilder builder)
        {
            if (_solrServers == null)
                return;

            var cores = new List<SolrCore>();

            foreach (SolrServerElement server in _solrServers)
            {
                var solrCore = GetCoreFrom(server);
                cores.Add(solrCore);
            }

            foreach (var core in cores)
            {
                RegisterCore(core, builder);
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

        #endregion
    }
}