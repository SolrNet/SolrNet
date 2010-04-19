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

            For<IHttpWebRequestFactory>().Use<HttpWebRequestFactory>();

            For<ISolrConnection>().Use<SolrConnection>()
                .Ctor<string>("serverURL").Is(solrURL);

            For<ISolrDocumentPropertyVisitor>().Use<DefaultDocumentVisitor>();
            For<ISolrFieldParser>().Use<DefaultFieldParser>();
            
            For(typeof(ISolrDocumentResponseParser<>)).Use(typeof(SolrDocumentResponseParser<>));
            For(typeof(ISolrDocumentResponseParser<>)).Use(typeof(SolrDocumentResponseParser<>));

            For<ISolrDocumentResponseParser<Dictionary<string, object>>>()
                .Use<SolrDictionaryDocumentResponseParser>();              
            
            For(typeof(ISolrDocumentIndexer<>)).Use(typeof(SolrDocumentIndexer<>));
            For<ISolrFieldSerializer>().Use<DefaultFieldSerializer>();

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
                For(typeof (ISolrResponseParser<>)).Use(p);
            }

            For(typeof(ISolrQueryResultParser<>)).Use(typeof(SolrQueryResultParser<>));
            For(typeof(ISolrQueryExecuter<>)).Use(typeof(SolrQueryExecuter<>));
            For(typeof(ISolrDocumentSerializer<>)).Use(typeof(SolrDocumentSerializer<>));

            For(typeof(ISolrDocumentSerializer<Dictionary<string,object>>)).Use(typeof(SolrDictionarySerializer));
            For(typeof(ISolrBasicOperations<>)).Use(typeof(SolrBasicServer<>));
            For(typeof(ISolrBasicReadOnlyOperations<>)).Use(typeof(SolrBasicServer<>));
            For(typeof(ISolrOperations<>)).Use(typeof(SolrServer<>));
            For(typeof(ISolrReadOnlyOperations<>)).Use(typeof(SolrServer<>));
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