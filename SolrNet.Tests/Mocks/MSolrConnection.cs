using System;
using System.Collections.Generic;
using System.IO;
using Moroco;
using SolrNet.Impl;

namespace SolrNet.Tests.Mocks {
    public class MSolrConnection : ISolrConnection {
        public MFunc<string, string, string> post;
        public MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string> postStream;
        public MFunc<string, IEnumerable<KeyValuePair<string, string>>, string> get;

        public ISolrQueryResponse Post(string relativeUrl, string s) {
            if (post == null)
                throw new NotImplementedException(string.Format("Post called with\n{0}\n{1}", relativeUrl, s));
            var solrResponse = new SolrQueryResponse(post.Invoke(relativeUrl, s));
            solrResponse.MetaData.OriginalQuery = s;
            return solrResponse;
        }

        public ISolrQueryResponse PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters) {
            if (postStream == null)
                throw new NotImplementedException();
            var solrResponse = new SolrQueryResponse(postStream.Invoke(relativeUrl, contentType, content, getParameters));
            solrResponse.MetaData.OriginalQuery = string.Empty;
            return solrResponse;
        }

        public ISolrQueryResponse Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) {
            var solrResponse = new SolrQueryResponse(get.Invoke(relativeUrl, parameters));
            solrResponse.MetaData.OriginalQuery = string.Empty;
            return solrResponse;
        }
    }
}