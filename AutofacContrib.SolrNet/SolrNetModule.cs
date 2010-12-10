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

using Autofac;
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

namespace AutofacContrib.SolrNet {
    public class SolrNetModule : Module {
        private readonly string ServerUrl;

        public SolrNetModule(string serverUrl) {
            ServerUrl = serverUrl;
        }

        public IReadOnlyMappingManager Mapper { get; set; }

        protected override void Load(ContainerBuilder builder)
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
            })
                builder.RegisterGeneric(p).As(typeof(ISolrResponseParser<>));
            builder.RegisterType<HeaderResponseParser<string>>().As<ISolrHeaderResponseParser>();
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
            builder.RegisterGeneric(typeof (SolrBasicServer<>))
                .As(typeof (ISolrBasicOperations<>), typeof (ISolrBasicReadOnlyOperations<>))
                .SingleInstance();
            builder.RegisterGeneric(typeof(SolrServer<>))
                .As(typeof(ISolrOperations<>), typeof(ISolrReadOnlyOperations<>))
                .SingleInstance();
            builder.RegisterType<SolrSchemaParser>().As<ISolrSchemaParser>();
            builder.RegisterType<MappingValidator>().As<IMappingValidator>();
        }
    }
}