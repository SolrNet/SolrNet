using System;
using System.Collections.Generic;
using System.IO;
using Moroco;
using System.Threading.Tasks;
using System.Threading;
using SolrNet.Impl;

namespace SolrNet.Tests.Mocks {
    public class MSolrConnection : ISolrConnection {
        public MFunc<string, string, SolrQueryResponse> post;
        public MFunc<string, string, Stream, IEnumerable<KeyValuePair<string,string>>, SolrQueryResponse> postStream;
        public MFunc<string, IEnumerable<KeyValuePair<string, string>>, SolrQueryResponse> get;

        public MFunc<string, string, Task<SolrQueryResponse>> postAsync;
        public MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, Task<SolrQueryResponse>> postStreamAsync;
        public MFunc<string, IEnumerable<KeyValuePair<string, string>>,Task<SolrQueryResponse>> getAsync;

        public SolrQueryResponse Post(string relativeUrl, string s) {
            if (post == null)
                throw new NotImplementedException(string.Format("Post called with\n{0}\n{1}", relativeUrl, s));
            return post.Invoke(relativeUrl, s);
        }

        public SolrQueryResponse PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters) {
            if (postStream == null)
                throw new NotImplementedException();
            return postStream.Invoke(relativeUrl, contentType, content, getParameters);
        }

        public SolrQueryResponse Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) {
            return get.Invoke(relativeUrl, parameters);
        }

        public Task<SolrQueryResponse> PostAsync(string relativeUrl, string s)
        {
            if (post == null)
                throw new NotImplementedException(string.Format("Post called with\n{0}\n{1}", relativeUrl, s));
            return postAsync.Invoke(relativeUrl, s);
        }

        public Task<SolrQueryResponse> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            if (postStream == null)
                throw new NotImplementedException();
            return postStreamAsync.Invoke(relativeUrl, contentType, content, getParameters);
        }

        public Task<SolrQueryResponse> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            return getAsync.Invoke(relativeUrl, parameters);
        }
    }
}
