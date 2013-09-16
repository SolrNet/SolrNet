using System;
using System.Collections.Generic;
using Moroco;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Schema;

namespace SampleSolrApp.Tests {
    public class MSolrReadOnlyOperations<T> : ISolrReadOnlyOperations<T> {
        public MFunc<ISolrQuery, QueryOptions, SolrQueryResults<T>> query;

        public SolrQueryResults<T> Query(ISolrQuery q, QueryOptions options) {
            return query.Invoke(q, options);
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options) {
            throw new NotImplementedException();
        }

        public ResponseHeader Ping() {
            throw new NotImplementedException();
        }

        public SolrSchema GetSchema() {
            throw new NotImplementedException();
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options) {
            throw new NotImplementedException();
        }

        public SolrQueryResults<T> Query(string q) {
            throw new NotImplementedException();
        }

        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders) {
            throw new NotImplementedException();
        }

        public SolrQueryResults<T> Query(string q, QueryOptions options) {
            throw new NotImplementedException();
        }

        public SolrQueryResults<T> Query(ISolrQuery q) {
            throw new NotImplementedException();
        }

        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders) {
            throw new NotImplementedException();
        }

        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets) {
            throw new NotImplementedException();
        }
    }
}