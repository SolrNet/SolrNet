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
        private static readonly object myLockContext = new object();
        private static IDictionary<string, bool> types = new Dictionary<string, bool>();

        static Startup() {
            InitContainer();
        }

        public static void InitContainer() {
            types.Clear();
            ServiceLocator.SetLocatorProvider(() => Container);
            Container.Clear();

            Container.Register<IReadOnlyMappingManager>(c => new MemoizingMappingManager(new AttributesMappingManager()));
            Container.Register<ISolrFieldParser>(c => new DefaultFieldParser());
            Container.Register<ISolrFieldSerializer>(c => new DefaultFieldSerializer());
            Container.Register<ISolrQuerySerializer>(c => new DefaultQuerySerializer(c.GetInstance<ISolrFieldSerializer>()));
            Container.Register<ISolrFacetQuerySerializer>(c => new DefaultFacetQuerySerializer(c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrFieldSerializer>()));
            Container.Register<ISolrDocumentPropertyVisitor>(c => new DefaultDocumentVisitor(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrFieldParser>()));

            //Container.Register<ISolrCache>(c => new HttpRuntimeCache());

            Container.Register<ISolrSchemaParser>(c => new SolrSchemaParser());
            Container.Register<ISolrHeaderResponseParser>(c => new HeaderResponseParser<string>());
            Container.RegisterAll<IValidationRule>(new List<Converter<IContainer, IValidationRule>> {
                c => new MappedPropertiesIsInSolrSchemaRule(), 
                c => new RequiredFieldsAreMappedRule(), 
                c => new UniqueKeyMatchesMappingRule()
            });
            Container.Register<IMappingValidator>(c => new MappingValidator(c.GetInstance<IReadOnlyMappingManager>(), Func.ToArray(c.GetAllInstances<IValidationRule>())));
        }

        private static IEnumerable<IValidationRule> ChooseValidationRules() {
            yield return new MappedPropertiesIsInSolrSchemaRule();
            yield return new RequiredFieldsAreMappedRule();
            yield return new UniqueKeyMatchesMappingRule();
        }

        /// <summary>
        /// Initializes SolrNet for the given server URL using the built-in container.
        /// </summary>
        /// <typeparam name="T">Document type.</typeparam>
        /// <param name="serverURL">Solr URL (i.e. "http://localhost:8983/solr")</param>
        public static void Init<T>(string serverURL) {
            InitCore<T>(null, serverURL);
        }

        /// <summary>
        /// Initializes SolrNet for the given core name and server URL using the built-in container.
        /// </summary>
        /// <typeparam name="T">Document type.</typeparam>
        /// <param name="serverURL">Solr URL (i.e. "http://localhost:8983/solr")</param>
        public static void InitCore<T>(string coreName, string serverURL)
        {
            var connection = new SolrConnection(serverURL) {
                //Cache = Container.GetInstance<ISolrCache>(),
            };

            InitCore<T>(coreName, connection);
        }

        /// <summary>
        /// Initializes SolrNet for the given solr connection with the built-in container.
        /// </summary>
        /// <typeparam name="T">Document type.</typeparam>
        /// <param name="connection">Solr connection.</param>
        public static void Init<T>(ISolrConnection connection) {
            InitCore<T>(null, connection);
        }

        /// <summary>
        /// Initializes SolrNet for the given core name and solr connection with the built-in container.
        /// </summary>
        /// <typeparam name="T">Document type.</typeparam>
        /// <param name="connection">Solr connection.</param>
        public static void InitCore<T>(string coreName, ISolrConnection connection) {
            lock (myLockContext) {
                if (!types.ContainsKey(typeof(T).FullName)) {
                    Container.Register<ISolrDocumentActivator<T>>(c => new SolrDocumentActivator<T>());
                    Container.Register(c => ChooseDocumentResponseParser<T>(c));
                    Container.Register(c => ChooseDocumentSerializer<T>(c));

                    Container.RegisterAll<ISolrResponseParser<T>>(new List<Converter<IContainer, ISolrResponseParser<T>>> {
                        c => new ResultsResponseParser<T>(c.GetInstance<ISolrDocumentResponseParser<T>>()),
                        c => new HeaderResponseParser<T>(),
                        c => new FacetsResponseParser<T>(),
                        c => new HighlightingResponseParser<T>(),
                        c => new MoreLikeThisResponseParser<T>(c.GetInstance<ISolrDocumentResponseParser<T>>()),
                        c => new SpellCheckResponseParser<T>(),
                        c => new StatsResponseParser<T>(),
                        c => new CollapseResponseParser<T>()
                    });
                    Container.Register<ISolrQueryResultParser<T>>(c => new SolrQueryResultParser<T>(Func.ToArray(Container.GetAllInstances<ISolrResponseParser<T>>())));

                    types.Add(typeof(T).FullName, true);
                }
            }
             
            var connectionKey = (string.IsNullOrEmpty(coreName))
                ? string.Format("{0}.{1}.{2}", typeof(SolrConnection), typeof(T), connection.GetType())
                : string.Format("{0}.{1}.{2}.{3}", coreName, typeof(SolrConnection), typeof(T), connection.GetType());
            Container.Register(connectionKey, c => connection);

            var coreNameKey = (string.IsNullOrEmpty(coreName)) ? null : coreName;
            Container.Register<ISolrQueryExecuter<T>>(coreNameKey, c => new SolrQueryExecuter<T>(c.GetInstance<ISolrQueryResultParser<T>>(), connection, c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrFacetQuerySerializer>()));
            Container.Register<ISolrBasicOperations<T>>(coreNameKey, c => new SolrBasicServer<T>(connection, c.GetInstance<ISolrQueryExecuter<T>>(coreNameKey), c.GetInstance<ISolrDocumentSerializer<T>>(), c.GetInstance<ISolrSchemaParser>(), c.GetInstance<ISolrHeaderResponseParser>(), c.GetInstance<ISolrQuerySerializer>()));
            Container.Register<ISolrBasicReadOnlyOperations<T>>(coreNameKey, c => new SolrBasicServer<T>(connection, c.GetInstance<ISolrQueryExecuter<T>>(coreNameKey), c.GetInstance<ISolrDocumentSerializer<T>>(), c.GetInstance<ISolrSchemaParser>(), c.GetInstance<ISolrHeaderResponseParser>(), c.GetInstance<ISolrQuerySerializer>()));
            Container.Register<ISolrOperations<T>>(coreNameKey, c => new SolrServer<T>(c.GetInstance<ISolrBasicOperations<T>>(coreNameKey), Container.GetInstance<IReadOnlyMappingManager>(), Container.GetInstance<IMappingValidator>()));
            Container.Register<ISolrReadOnlyOperations<T>>(coreNameKey, c => new SolrServer<T>(c.GetInstance<ISolrBasicOperations<T>>(coreNameKey), Container.GetInstance<IReadOnlyMappingManager>(), Container.GetInstance<IMappingValidator>()));
        }

        private static ISolrDocumentSerializer<T> ChooseDocumentSerializer<T>(IServiceLocator c) {
            return (typeof (T) == typeof (Dictionary<string, object>))
                   ? (ISolrDocumentSerializer<T>) new SolrDictionarySerializer(c.GetInstance<ISolrFieldSerializer>())
                   : new SolrDocumentSerializer<T>(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrFieldSerializer>());
        }

        private static ISolrDocumentResponseParser<T> ChooseDocumentResponseParser<T>(IServiceLocator c) {
            return (typeof(T) == typeof(Dictionary<string, object>))
                   ? (ISolrDocumentResponseParser<T>) new SolrDictionaryDocumentResponseParser(c.GetInstance<ISolrFieldParser>())
                   : new SolrDocumentResponseParser<T>(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrDocumentPropertyVisitor>(), c.GetInstance<ISolrDocumentActivator<T>>());
        }
    }
}