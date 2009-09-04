#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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

using Microsoft.Practices.ServiceLocation;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
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

            var propertyVisitor = new DefaultDocumentVisitor(mapper, fieldParser);
            Container.Register<ISolrDocumentPropertyVisitor>(c => propertyVisitor);

            var rng = new RNG();
            Container.Register<IRNG>(c => rng);
        }

        /// <summary>
        /// Initializes SolrNet with the built-in container
        /// </summary>
        /// <typeparam name="T">Document type</typeparam>
        /// <param name="serverURL">Solr URL (i.e. "http://localhost:8983/solr")</param>
        public static void Init<T>(string serverURL) where T: new() {
            var connection = new SolrConnection(serverURL);

            Init<T>(connection);
        }

        /// <summary>
        /// Initializes SolrNet with the built-in container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        public static void Init<T>(ISolrConnection connection) where T: new() {
            var connectionKey = string.Format("{0}.{1}.{2}", typeof(SolrConnection), typeof(T), connection.GetType());
            Container.Register(connectionKey, c => connection);

            Container.Register<ISolrDocumentResponseParser<T>>(c => new SolrDocumentResponseParser<T>(Container.GetInstance<IReadOnlyMappingManager>(), Container.GetInstance<ISolrDocumentPropertyVisitor>()));
            Container.Register<ISolrDocumentIndexer<T>>(c => new SolrDocumentIndexer<T>(Container.GetInstance<IReadOnlyMappingManager>()));

            Container.Register<ISolrResponseParser<T>>(typeof(ResultsResponseParser<T>).FullName, c => new ResultsResponseParser<T>(c.GetInstance<ISolrDocumentResponseParser<T>>()));
            Container.Register<ISolrResponseParser<T>>(typeof (HeaderResponseParser<T>).FullName, c => new HeaderResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(FacetsResponseParser<T>).FullName, c => new FacetsResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(HighlightingResponseParser<T>).FullName, c => new HighlightingResponseParser<T>(c.GetInstance<ISolrDocumentIndexer<T>>()));
            Container.Register<ISolrResponseParser<T>>(typeof(MoreLikeThisResponseParser<T>).FullName, c => new MoreLikeThisResponseParser<T>(c.GetInstance<ISolrDocumentResponseParser<T>>(), c.GetInstance<ISolrDocumentIndexer<T>>()));
            Container.Register<ISolrResponseParser<T>>(typeof(SpellCheckResponseParser<T>).FullName, c => new SpellCheckResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(StatsResponseParser<T>).FullName, c => new StatsResponseParser<T>());
            Container.Register<ISolrResponseParser<T>>(typeof(CollapseResponseParser<T>).FullName, c => new CollapseResponseParser<T>());

            var resultParser = new SolrQueryResultParser<T>(Func.ToArray(Container.GetAllInstances<ISolrResponseParser<T>>()));
            Container.Register<ISolrQueryResultParser<T>>(c => resultParser);

            var queryExecuter = new SolrQueryExecuter<T>(connection, resultParser);
            Container.Register<ISolrQueryExecuter<T>>(c => queryExecuter);

            var documentSerializer = new SolrDocumentSerializer<T>(Container.GetInstance<IReadOnlyMappingManager>(), Container.GetInstance<ISolrFieldSerializer>());
            Container.Register<ISolrDocumentSerializer<T>>(c => documentSerializer);

            var basicServer = new SolrBasicServer<T>(connection, queryExecuter, documentSerializer);
            Container.Register<ISolrBasicOperations<T>>(c => basicServer);
            Container.Register<ISolrBasicReadOnlyOperations<T>>(c => basicServer);

            var server = new SolrServer<T>(basicServer, Container.GetInstance<IReadOnlyMappingManager>());
            Container.Register<ISolrOperations<T>>(c => server);
            Container.Register<ISolrReadOnlyOperations<T>>(c => server);
            
        }
    }
}