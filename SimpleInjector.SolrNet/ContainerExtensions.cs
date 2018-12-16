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
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleInjector.SolrNet
{
    public static class ContainerExtensions
    {
        public static Container AddSolrNet(this Container container, string url)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return container.AddSolrNet(new SolrCore[] { new SolrCore(null, null, url) }, null);
        }

        public static Container AddSolrNet(this Container container, string url, Action<SolrNetOptions> setupAction) => AddSolrNet(container, new SolrCore[] { new SolrCore(null, null, url) }, setupAction);

        private static Container AddSolrNet(this Container container, IEnumerable<SolrCore> cores, Action<SolrNetOptions> setupAction)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Register<IReadOnlyMappingManager>(() => new MemoizingMappingManager(new AttributesMappingManager()), Lifestyle.Singleton);
            container.Register<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
            container.Register<ISolrFieldParser, DefaultFieldParser>();
            container.Register(typeof(ISolrDocumentActivator<>), typeof(SolrDocumentActivator<>));
            container.RegisterConditional(typeof(ISolrDocumentResponseParser<>), typeof(SolrDocumentResponseParser<>), c => !c.Handled);

            container.Register<ISolrDocumentResponseParser<Dictionary<string, object>>, SolrDictionaryDocumentResponseParser>();
            container.Register<ISolrFieldSerializer, DefaultFieldSerializer>();
            container.Register<ISolrQuerySerializer, DefaultQuerySerializer>();
            container.Register<ISolrFacetQuerySerializer, DefaultFacetQuerySerializer>();
            container.Register(typeof(ISolrAbstractResponseParser<>), typeof(DefaultResponseParser<>));
            container.Collection.Register(typeof(ISolrAbstractResponseParser<>), new[] { typeof(DefaultResponseParser<>) });
            container.Register<ISolrHeaderResponseParser, HeaderResponseParser<string>>();
            container.Register<ISolrExtractResponseParser, ExtractResponseParser>();
            var p = new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
                typeof(MultivaluedMappedToCollectionRule),
            };
            container.Collection.Register(typeof(IValidationRule), p);
            container.Register(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>), typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
            container.RegisterConditional(typeof(ISolrDocumentSerializer<>), typeof(SolrDocumentSerializer<>), c => !c.Handled);
            container.Register<ISolrDocumentSerializer<Dictionary<string, object>>, SolrDictionarySerializer>();

            container.Register<ISolrSchemaParser, SolrSchemaParser>();
            container.Register<ISolrDIHStatusParser, SolrDIHStatusParser>();
            container.Register<IMappingValidator, MappingValidator>();

            if (!cores.Any()) return container;

            if (cores.Count() > 1)
            {
                throw new NotImplementedException("Need to add multicore support");
            }

            var connection = new AutoSolrConnection(cores.Single().Url);
            //Bind single type to a single url, prevent breaking existing functionality
            container.Register<ISolrConnection>(() => connection, Lifestyle.Singleton);
            container.Register(typeof(ISolrQueryExecuter<>), typeof(SolrQueryExecuter<>));
            container.Register(typeof(ISolrBasicOperations<>), typeof(SolrBasicServer<>));
            container.Register(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrBasicServer<>));
            container.Register(typeof(ISolrOperations<>), typeof(SolrServer<>));
            container.Register(typeof(ISolrReadOnlyOperations<>), typeof(SolrServer<>));

            if (setupAction != null)
            {
                var options = new SolrNetOptions(connection.HttpClient);
                //allow for custom headers to be injected.
                setupAction(options);
            }

            return container;
        }
    }
}
