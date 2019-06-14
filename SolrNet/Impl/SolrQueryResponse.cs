using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl
{
    /// <summary>
    /// Wraps the solr response and meta data relating to the response
    /// </summary>
    public class SolrQueryResponse<T> : IDisposable
    {
        /// <summary>
        /// The string response received from solr
        /// </summary>
        public T Response { get; private set; }

        /// <summary>
        /// Provides metadata on the solr response
        /// Getter will never return null.
        /// </summary>
        public SolrResponseMetaData MetaData { get; private set; }

        /// <summary>
        /// Response object wrapping the Solr string response and other relevant information
        /// </summary>
        /// <param name="solrResponse">The string response from Solr</param>
        /// <param name="metaData">Metadata on the Solr response</param>
        public SolrQueryResponse(T solrResponse, SolrResponseMetaData metaData = null) {
            Response = solrResponse;
            MetaData = metaData ?? new SolrResponseMetaData();
        }

        /// <summary>
        /// Disposable support
        /// </summary>
        public void Dispose()
        {
            if (Response is IDisposable disposable)
                disposable.Dispose();
        }
    }

    /// <summary>
    /// Most commonly used solr response type is of string, thus the concrete implementation
    /// </summary>
    public class SolrQueryResponse : SolrQueryResponse<string>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="solrResponse"></param>
        /// <param name="metaData"></param>
        public SolrQueryResponse(string solrResponse, SolrResponseMetaData metaData = null) : base(solrResponse, metaData) { }
    }
}
