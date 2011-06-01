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

namespace Ninject.Integration.SolrNet {
    public class SolrNetModule : NinjectModule {
        private readonly string serverURL;
        public IReadOnlyMappingManager Mapper { get; set; }

        public SolrNetModule(string serverURL) {
            this.serverURL = serverURL;
        }

        public override void Load() {
            var mapper = Mapper ?? new MemoizingMappingManager(new AttributesMappingManager());
            Bind<IReadOnlyMappingManager>().ToConstant(mapper);
            //Bind<ISolrCache>().To<HttpRuntimeCache>();
            Bind<ISolrDocumentPropertyVisitor>().To<DefaultDocumentVisitor>();
            Bind<ISolrFieldParser>().To<DefaultFieldParser>();
            Bind(typeof (ISolrDocumentActivator<>)).To(typeof (SolrDocumentActivator<>));
            Bind(typeof(ISolrDocumentResponseParser<>)).To(typeof(SolrDocumentResponseParser<>));
            Bind<ISolrFieldSerializer>().To<DefaultFieldSerializer>();
            Bind<ISolrQuerySerializer>().To<DefaultQuerySerializer>();
            Bind<ISolrFacetQuerySerializer>().To<DefaultFacetQuerySerializer>();
            foreach (var p in new[] {
                typeof(ResultsResponseParser<>),
                typeof(HeaderResponseParser<>),
                typeof(FacetsResponseParser<>),
                typeof(HighlightingResponseParser<>),
                typeof(MoreLikeThisResponseParser<>),
                typeof(SpellCheckResponseParser<>),
                typeof(StatsResponseParser<>),
                typeof(CollapseResponseParser<>),
                typeof(GroupingResponseParser<>)
            })
                Bind(typeof(ISolrResponseParser<>)).To(p);
            Bind<ISolrHeaderResponseParser>().To<HeaderResponseParser<string>>();
            Bind<ISolrExtractResponseParser>().To<ExtractResponseParser>();
            foreach (var p in new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
            })
                Bind<IValidationRule>().To(p);
            Bind<ISolrConnection>().ToConstant(new SolrConnection(serverURL));
            Bind(typeof (ISolrQueryResultParser<>)).To(typeof (SolrQueryResultParser<>));
            Bind(typeof(ISolrQueryExecuter<>)).To(typeof(SolrQueryExecuter<>));
            Bind(typeof(ISolrDocumentSerializer<>)).To(typeof(SolrDocumentSerializer<>));
            Bind(typeof(ISolrBasicOperations<>)).To(typeof(SolrBasicServer<>));
            Bind(typeof(ISolrBasicReadOnlyOperations<>)).To(typeof(SolrBasicServer<>));
            Bind(typeof(ISolrOperations<>)).To(typeof(SolrServer<>));
            Bind(typeof(ISolrReadOnlyOperations<>)).To(typeof(SolrServer<>));
            Bind<ISolrSchemaParser>().To<SolrSchemaParser>();
            Bind<ISolrDIHStatusParser>().To<SolrDIHStatusParser>();
            Bind<IMappingValidator>().To<MappingValidator>();
        }
    }
}