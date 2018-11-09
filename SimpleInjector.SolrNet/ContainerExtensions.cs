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
            container.Register<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>(Lifestyle.Transient);
            container.Register<ISolrFieldParser, DefaultFieldParser>(Lifestyle.Transient);
            container.Register(typeof(ISolrDocumentActivator<>), typeof(SolrDocumentActivator<>), Lifestyle.Transient);
            container.RegisterConditional(typeof(ISolrDocumentResponseParser<>), typeof(SolrDocumentResponseParser<>), c => !c.Handled);

            container.Register<ISolrDocumentResponseParser<Dictionary<string, object>>, SolrDictionaryDocumentResponseParser>(Lifestyle.Transient);
            container.Register<ISolrFieldSerializer, DefaultFieldSerializer>(Lifestyle.Transient);
            container.Register<ISolrQuerySerializer, DefaultQuerySerializer>(Lifestyle.Transient);
            container.Register<ISolrFacetQuerySerializer, DefaultFacetQuerySerializer>(Lifestyle.Transient);
            container.Register(typeof(ISolrAbstractResponseParser<>), typeof(DefaultResponseParser<>), Lifestyle.Transient);
            container.Collection.Register(typeof(ISolrAbstractResponseParser<>), new[] { typeof(DefaultResponseParser<>) });
            container.Register<ISolrHeaderResponseParser, HeaderResponseParser<string>>(Lifestyle.Transient);
            container.Register<ISolrExtractResponseParser, ExtractResponseParser>(Lifestyle.Transient);
            var p = new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
                typeof(MultivaluedMappedToCollectionRule),
            };
            container.Collection.Register(typeof(IValidationRule), p);
            container.Register(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>), typeof(SolrMoreLikeThisHandlerQueryResultsParser<>), Lifestyle.Transient);
            container.RegisterConditional(typeof(ISolrDocumentSerializer<>), typeof(SolrDocumentSerializer<>), c => !c.Handled);
            container.Register<ISolrDocumentSerializer<Dictionary<string, object>>, SolrDictionarySerializer>(Lifestyle.Transient);

            container.Register<ISolrSchemaParser, SolrSchemaParser>(Lifestyle.Transient);
            container.Register<ISolrDIHStatusParser, SolrDIHStatusParser>(Lifestyle.Transient);
            container.Register<IMappingValidator, MappingValidator>(Lifestyle.Transient);

            if (!cores.Any()) return container;

            if (cores.Count() > 1)
            {
                throw new NotImplementedException("Need to add multicore support");
            }

            var connection = new AutoSolrConnection(cores.Single().Url);
            //Bind single type to a single url, prevent breaking existing functionality
            container.Register<ISolrConnection>(() => connection, Lifestyle.Singleton);
            container.Register(typeof(ISolrQueryExecuter<>), typeof(SolrQueryExecuter<>), Lifestyle.Transient);
            container.Register(typeof(ISolrBasicOperations<>), typeof(SolrBasicServer<>), Lifestyle.Transient);
            container.Register(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrBasicServer<>), Lifestyle.Transient);
            container.Register(typeof(ISolrOperations<>), typeof(SolrServer<>), Lifestyle.Transient);
            container.Register(typeof(ISolrReadOnlyOperations<>), typeof(SolrServer<>), Lifestyle.Transient);
            
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
