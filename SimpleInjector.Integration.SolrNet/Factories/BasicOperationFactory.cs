using SolrNet;
using SolrNet.Impl;
using SolrNet.Schema;
using System;

namespace SimpleInjector.Integration.SolrNet.Factories
{
    class BasicOperationFactory
    {
        private readonly Container _Container;

        public BasicOperationFactory(Container container)
        {
            _Container = container;
        }

        public object Create(SolrCore core)
        {
            var outputType = typeof(SolrBasicServer<>).MakeGenericType(core.DocumentType);
            var queryExecuterType = typeof(ISolrQueryExecuter<>).MakeGenericType(core.DocumentType);
            var docSerializerType = typeof(ISolrDocumentSerializer<>).MakeGenericType(core.DocumentType);

            var output = Activator.CreateInstance(type: outputType,
                args: new[] {
                        new SolrConnection(core.ConnectionUrl),
                        _Container.GetInstance(queryExecuterType),
                        _Container.GetInstance(docSerializerType),
                        _Container.GetInstance<ISolrSchemaParser>(),
                        _Container.GetInstance<ISolrHeaderResponseParser>(),
                        _Container.GetInstance<ISolrQuerySerializer>(),
                        _Container.GetInstance<ISolrDIHStatusParser>(),
                        _Container.GetInstance<ISolrExtractResponseParser>()
                });

            return output;
        }
    }
}
