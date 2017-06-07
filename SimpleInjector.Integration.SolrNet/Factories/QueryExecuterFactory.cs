using SolrNet.Impl;
using System;

namespace SimpleInjector.Integration.SolrNet.Factories
{
    class QueryExecuterFactory
    {
        private readonly Container _Container;

        public QueryExecuterFactory(Container container)
        {
            _Container = container;
        }

        public object Create(SolrCore core)
        {
            var outputType = typeof(SolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            var responseParserType = typeof(ISolrAbstractResponseParser<>).MakeGenericType(core.DocumentType);
            var queryResultParserType = typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>).MakeGenericType(core.DocumentType);

            var output = Activator.CreateInstance(type: outputType,
                args: new[] {
                        _Container.GetInstance(responseParserType),
                        new SolrConnection(core.ConnectionUrl),
                        _Container.GetInstance<ISolrQuerySerializer>(),
                        _Container.GetInstance<ISolrFacetQuerySerializer>(),
                        _Container.GetInstance(queryResultParserType)
                });

            return output;
        }
    }
}
