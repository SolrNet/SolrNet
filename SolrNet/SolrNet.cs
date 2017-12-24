using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace SolrNet
{
    /// <summary>
    /// SolrNet helper
    /// </summary>
    public static class SolrNet {
        /// <summary>
        /// Gets basic server instance
        /// </summary>
        public static ISolrBasicOperations<T> GetBasicServer<T>(string url, bool isPostConnection) {
            var connection = new SolrConnection(url);
            return !isPostConnection ? GetBasicServer<T>(connection) : GetBasicServer<T>(new PostSolrConnection(connection, url));
        }

        /// <summary>
        /// Gets server instance
        /// </summary>
        public static ISolrOperations<T> GetServer<T>(string url, bool isPostConnection) {
            var connection = new SolrConnection(url);
            return !isPostConnection ? GetServer<T>(connection) : GetServer<T>(new PostSolrConnection(connection, url));
        }

        /// <summary>
        /// Gets basic server instance
        /// </summary>
        public static ISolrBasicOperations<T> GetBasicServer<T>(ISolrConnection connection) {
            ISolrFieldParser fieldParser = new DefaultFieldParser();
            IReadOnlyMappingManager mapper = new MemoizingMappingManager(new AttributesMappingManager());
            ISolrDocumentPropertyVisitor visitor = new DefaultDocumentVisitor(mapper, fieldParser);

            ISolrDocumentResponseParser<T> parser;
            if (typeof (T) == typeof (Dictionary<string, object>))
                parser = (ISolrDocumentResponseParser<T>) new SolrDictionaryDocumentResponseParser(fieldParser);
            else
                parser = new SolrDocumentResponseParser<T>(mapper, visitor, new SolrDocumentActivator<T>());

            ISolrAbstractResponseParser<T> resultParser = new DefaultResponseParser<T>(parser);

            ISolrFieldSerializer fieldSerializer = new DefaultFieldSerializer();
            ISolrQuerySerializer querySerializer = new DefaultQuerySerializer(fieldSerializer);
            ISolrFacetQuerySerializer facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, fieldSerializer);
            // validate why only this?
            ISolrMoreLikeThisHandlerQueryResultsParser<T> mlthResultParser = new SolrMoreLikeThisHandlerQueryResultsParser<T>(new[] {resultParser});

            ISolrQueryExecuter<T> executor = new SolrQueryExecuter<T>(resultParser, connection, querySerializer, facetQuerySerializer, mlthResultParser);

            ISolrDocumentSerializer<T> documentSerializer;

            if (typeof (T) == typeof (Dictionary<string, object>))
                documentSerializer = (ISolrDocumentSerializer<T>) new SolrDictionarySerializer(fieldSerializer);
            else
                documentSerializer = new SolrDocumentSerializer<T>(mapper, fieldSerializer);

            ISolrSchemaParser schemaParser = new SolrSchemaParser();
            ISolrHeaderResponseParser headerParser = new HeaderResponseParser<T>();
            ISolrDIHStatusParser dihStatusParser = new SolrDIHStatusParser();
            ISolrExtractResponseParser extractResponseParser = new ExtractResponseParser(headerParser);

            ISolrBasicOperations<T> basicServer = new SolrBasicServer<T>(connection, executor, documentSerializer, schemaParser, headerParser, querySerializer, dihStatusParser, extractResponseParser);

            return basicServer;
        }

        /// <summary>
        /// Gets server instance
        /// </summary>
        public static ISolrOperations<T> GetServer<T>(ISolrConnection connection) {
            IReadOnlyMappingManager mapper = new MemoizingMappingManager(new AttributesMappingManager());

            IReadOnlyMappingManager mappingManager = mapper;
            IMappingValidator mappingValidator = new MappingValidator(mapper, new IValidationRule[] {
                new MappedPropertiesIsInSolrSchemaRule(),
                new RequiredFieldsAreMappedRule(),
                new UniqueKeyMatchesMappingRule(),
                new MultivaluedMappedToCollectionRule()
            });

            var basicServer = GetBasicServer<T>(connection);
            ISolrOperations<T> server = new SolrServer<T>(basicServer, mappingManager, mappingValidator);

            return server;
        }
    }
}
