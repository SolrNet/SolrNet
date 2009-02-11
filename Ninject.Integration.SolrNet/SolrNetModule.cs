using Ninject.Core;
using SolrNet;
using SolrNet.Utils;

namespace Ninject.Integration.SolrNet {
    public class SolrNetModule : StandardModule {
        private readonly string serverURL;

        public SolrNetModule(string serverURL) {
            this.serverURL = serverURL;
        }

        public override void Load() {
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            Bind<IReadOnlyMappingManager>().ToConstant(mapper);
            Bind<IRNG>().To<RNG>();
            Bind<IListRandomizer>().To<ListRandomizer>();
            Bind<ISolrConnection>().ToConstant(new SolrConnection(serverURL));
            Bind(typeof (ISolrQueryResultParser<>)).To(typeof (SolrQueryResultParser<>));
            Bind(typeof(ISolrQueryExecuter<>)).To(typeof(SolrQueryExecuter<>));
            Bind(typeof(ISolrDocumentSerializer<>)).To(typeof(SolrDocumentSerializer<>));
            Bind(typeof(ISolrBasicOperations<>)).To(typeof(SolrBasicServer<>));
            Bind(typeof(ISolrBasicReadOnlyOperations<>)).To(typeof(SolrBasicServer<>));
            Bind(typeof(ISolrOperations<>)).To(typeof(SolrServer<>));
            Bind(typeof(ISolrReadOnlyOperations<>)).To(typeof(SolrServer<>));
        }
    }
}