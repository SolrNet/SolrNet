using System;
using System.Collections.Generic;
using HttpWebAdapters;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using StructureMap.Configuration.DSL;

namespace Structuremap.SolrNetIntegration
{
    public class SolrNetRegistry : global::StructureMap.Configuration.DSL.Registry
    {
        
        public SolrNetRegistry(string solrURL, IReadOnlyMappingManager mappingManager)
        {          
            ValidateUrl(solrURL);

            For<IReadOnlyMappingManager>().Use(mappingManager);
            For<ISolrCache>().Use<HttpRuntimeCache>();

            For<ISolrConnection>().Use<SolrConnection>()
                .Ctor<string>("serverURL").Is(solrURL);

            For(typeof(ISolrDocumentResponseParser<>)).Use(typeof(SolrDocumentResponseParser<>));

            For<ISolrDocumentResponseParser<Dictionary<string, object>>>()
                .Use<SolrDictionaryDocumentResponseParser>()
                .Ctor<ISolrFieldParser>("fieldParser").Is(i => i.TheInstanceNamed(typeof(InferringFieldParser).Name));

            For(typeof(ISolrDocumentIndexer<>)).Use(typeof(SolrDocumentIndexer<>));

            foreach (var p in new[] {
                                        typeof(ResultsResponseParser<>),
                                        typeof(HeaderResponseParser<>),
                                        typeof(FacetsResponseParser<>),
                                        typeof(HighlightingResponseParser<>),
                                        typeof(MoreLikeThisResponseParser<>),
                                        typeof(SpellCheckResponseParser<>),
                                        typeof(StatsResponseParser<>),
                                        typeof(CollapseResponseParser<>),
                                    })
            {
                For(typeof(ISolrResponseParser<>)).Use(p);
            }

            For(typeof(ISolrQueryResultParser<>)).Use(typeof(SolrQueryResultParser<>));
            For(typeof(ISolrQueryExecuter<>)).Use(typeof(SolrQueryExecuter<>));

            For(typeof(ISolrDocumentSerializer<>)).Use(typeof(SolrDocumentSerializer<>));
            For(typeof(ISolrDocumentSerializer<Dictionary<string, object>>)).Use(typeof(SolrDictionarySerializer));

            For(typeof(ISolrBasicReadOnlyOperations<>)).Use(typeof(SolrBasicServer<>));
            For(typeof(ISolrBasicOperations<>)).Use(typeof(SolrBasicServer<>));
            For(typeof(ISolrReadOnlyOperations<>)).Use(typeof(SolrServer<>));
            For(typeof(ISolrOperations<>)).Use(typeof(SolrServer<>));

            For<ISolrFieldParser>().Use<DefaultFieldParser>();
            For<ISolrFieldParser>().Use<InferringFieldParser>().Named(typeof(InferringFieldParser).Name);

            For<ISolrFieldSerializer>().Use<DefaultFieldSerializer>();
            For<ISolrDocumentPropertyVisitor>().Use<DefaultDocumentVisitor>();
        }

        public SolrNetRegistry(string solrURL)
            : this(solrURL, new MemoizingMappingManager(new AttributesMappingManager()))
        {
        }


        private static void ValidateUrl(string url)
        {
            try
            {
                var u = new Uri(url);
                if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
                    throw new InvalidURLException("Only HTTP or HTTPS protocols are supported");
            }
            catch (ArgumentException e)
            {
                throw new InvalidURLException(string.Format("Invalid URL '{0}'", url), e);
            }
            catch (UriFormatException e)
            {
                throw new InvalidURLException(string.Format("Invalid URL '{0}'", url), e);
            }
        }
    }
}