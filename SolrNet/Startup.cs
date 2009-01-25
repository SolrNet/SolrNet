using SolrNet.Utils;

namespace SolrNet {
    public static class Startup {
        static Startup() {
            Container = new Container();
            Factory.Init(Container);
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            Container.Register<IReadOnlyMappingManager>(c => mapper);

            var rng = new RNG();
            Container.Register<IRNG>(c => rng);

            var randomizer = new ListRandomizer();
            Container.Register<IListRandomizer>(c => randomizer);

        }

        public static readonly Container Container;

        public static void Init<T>(string serverURL) where T: new() {
            var connection = new SolrConnection(serverURL);
            var connectionKey = string.Format("{0}.{1}.{2}", typeof(SolrConnection), typeof(T), serverURL);
            Container.Register<ISolrConnection>(connectionKey, c => connection);

            var resultParser = new SolrQueryResultParser<T>(Container.GetInstance<IReadOnlyMappingManager>());
            Container.Register<ISolrQueryResultParser<T>>(c => resultParser);

            var queryExecuter = new SolrQueryExecuter<T>(connection, resultParser, Container.GetInstance<IReadOnlyMappingManager>());
            Container.Register<ISolrQueryExecuter<T>>(c => queryExecuter);

            var documentSerializer = new SolrDocumentSerializer<T>(Container.GetInstance<IReadOnlyMappingManager>());
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