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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
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

namespace SolrNet {
    /// <summary>
    /// SolrNet initialization manager
    /// </summary>
    public static class Startup {
        public static readonly Container Container = new Container();

        static Startup() {
            InitContainer();
        }

        public static void InitContainer() {
            ServiceLocator.SetLocatorProvider(() => Container);
            Container.Clear();
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());

            Container.Register<IReadOnlyMappingManager>(c => mapper);

            var fieldParser = new DefaultFieldParser();
            Container.Register<ISolrFieldParser>(c => fieldParser);

            var fieldSerializer = new DefaultFieldSerializer();
            Container.Register<ISolrFieldSerializer>(c => fieldSerializer);

            Container.Register<ISolrQuerySerializer>(c => new DefaultQuerySerializer(c.GetInstance<ISolrFieldSerializer>()));
            Container.Register<ISolrFacetQuerySerializer>(c => new DefaultFacetQuerySerializer(c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrFieldSerializer>()));

            Container.Register<ISolrDocumentPropertyVisitor>(c => new DefaultDocumentVisitor(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrFieldParser>()));

            //var cache = new HttpRuntimeCache();
            //Container.Register<ISolrCache>(c => cache);

            var solrSchemaParser = new SolrSchemaParser();
            Container.Register<ISolrSchemaParser>(c => solrSchemaParser);

            var solrDIHStatusParser = new SolrDIHStatusParser();
            Container.Register<ISolrDIHStatusParser>(c => solrDIHStatusParser);

            var headerParser = new HeaderResponseParser<string>();
            Container.Register<ISolrHeaderResponseParser>(c => headerParser);

            var extractResponseParser = new ExtractResponseParser(headerParser);
            Container.Register<ISolrExtractResponseParser>(c => extractResponseParser);

            Container.Register<IValidationRule>(typeof(MappedPropertiesIsInSolrSchemaRule).FullName, c => new MappedPropertiesIsInSolrSchemaRule());
            Container.Register<IValidationRule>(typeof(RequiredFieldsAreMappedRule).FullName, c => new RequiredFieldsAreMappedRule());
            Container.Register<IValidationRule>(typeof(UniqueKeyMatchesMappingRule).FullName, c => new UniqueKeyMatchesMappingRule());
            Container.Register<IMappingValidator>(c => new MappingValidator(c.GetInstance<IReadOnlyMappingManager>(), c.GetAllInstances<IValidationRule>().ToArray()));
        }

        /// <summary>
        /// Initializes SolrNet with the built-in container
        /// </summary>
        /// <typeparam name="T">Document type</typeparam>
        /// <param name="serverURL">Solr URL (i.e. "http://localhost:8983/solr")</param>
        public static void Init<T>(string serverURL) {
            var connection = new SolrConnection(serverURL) {
                //Cache = Container.GetInstance<ISolrCache>(),
            };

            Init<T>(connection);
        }

        /// <summary>
        /// Initializes SolrNet with the built-in container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        public static void Init<T>(ISolrConnection connection) {
            var connectionKey = string.Format("{0}.{1}.{2}", typeof(SolrConnection), typeof(T), connection.GetType());
            Container.Register(connectionKey, c => connection);

            var activator = new SolrDocumentActivator<T>();
            Container.Register<ISolrDocumentActivator<T>>(c => activator);

            Container.Register(ChooseDocumentResponseParser<T>);

            Container.Register<ISolrResponseParser<T>>(typeof(ResultsResponseParser<T>).FullName, c => new ResultsResponseParser<T>(c.GetInstance<ISolrDocumentResponseParser<T>>()));
            Container.Register<ISolrResponseParser<T>>(typeof (HeaderResponseParser<T>).FullName, c => new HeaderResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(FacetsResponseParser<T>).FullName, c => new FacetsResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(HighlightingResponseParser<T>).FullName, c => new HighlightingResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(MoreLikeThisResponseParser<T>).FullName, c => new MoreLikeThisResponseParser<T>(c.GetInstance<ISolrDocumentResponseParser<T>>()));
            Container.Register<ISolrResponseParser<T>>(typeof(SpellCheckResponseParser<T>).FullName, c => new SpellCheckResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(TermsResponseParser<T>).FullName, c => new TermsResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(StatsResponseParser<T>).FullName, c => new StatsResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(CollapseResponseParser<T>).FullName, c => new CollapseResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(ClusterResponseParser<T>).FullName, c => new ClusterResponseParser<T>());


			Container.Register<ISolrResponseParser<T>>(typeof(GroupingResponseParser<T>).FullName, c => new GroupingResponseParser<T>(c.GetInstance<ISolrDocumentResponseParser<T>>()));

            Container.Register<ISolrQueryResultParser<T>>(c => new SolrQueryResultParser<T>(c.GetAllInstances<ISolrResponseParser<T>>().ToArray()));
            Container.Register<ISolrQueryExecuter<T>>(c => new SolrQueryExecuter<T>(c.GetInstance<ISolrQueryResultParser<T>>(), connection, c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrFacetQuerySerializer>()));

            Container.Register(ChooseDocumentSerializer<T>);

            Container.Register<ISolrBasicOperations<T>>(c => new SolrBasicServer<T>(connection, c.GetInstance<ISolrQueryExecuter<T>>(), c.GetInstance<ISolrDocumentSerializer<T>>(), c.GetInstance<ISolrSchemaParser>(), c.GetInstance<ISolrHeaderResponseParser>(), c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrDIHStatusParser>(), c.GetInstance<ISolrExtractResponseParser>()));
            Container.Register<ISolrBasicReadOnlyOperations<T>>(c => new SolrBasicServer<T>(connection, c.GetInstance<ISolrQueryExecuter<T>>(), c.GetInstance<ISolrDocumentSerializer<T>>(), c.GetInstance<ISolrSchemaParser>(), c.GetInstance<ISolrHeaderResponseParser>(), c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrDIHStatusParser>(), c.GetInstance<ISolrExtractResponseParser>()));

            Container.Register<ISolrOperations<T>>(c => new SolrServer<T>(c.GetInstance<ISolrBasicOperations<T>>(), Container.GetInstance<IReadOnlyMappingManager>(), Container.GetInstance<IMappingValidator>()));
            Container.Register<ISolrReadOnlyOperations<T>>(c => new SolrServer<T>(c.GetInstance<ISolrBasicOperations<T>>(), Container.GetInstance<IReadOnlyMappingManager>(), Container.GetInstance<IMappingValidator>()));
        }

        private static ISolrDocumentSerializer<T> ChooseDocumentSerializer<T>(IServiceLocator c) {
            if (typeof(T) == typeof(Dictionary<string, object>))
                return (ISolrDocumentSerializer<T>) new SolrDictionarySerializer(c.GetInstance<ISolrFieldSerializer>());
            return new SolrDocumentSerializer<T>(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrFieldSerializer>());
        }

        private static ISolrDocumentResponseParser<T> ChooseDocumentResponseParser<T>(IServiceLocator c) {
            if (typeof(T) == typeof(Dictionary<string, object>))
                return (ISolrDocumentResponseParser<T>) new SolrDictionaryDocumentResponseParser(c.GetInstance<ISolrFieldParser>());
            return new SolrDocumentResponseParser<T>(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrDocumentPropertyVisitor>(), c.GetInstance<ISolrDocumentActivator<T>>());
        }
    }
}
