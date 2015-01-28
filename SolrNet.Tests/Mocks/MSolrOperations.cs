using System;
using System.Collections.Generic;
using Moroco;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrNet.Tests.Mocks {
    public class MSolrOperations<T> : ISolrOperations<T> {
        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            throw new NotImplementedException();
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options) {
            throw new NotImplementedException();
        }

        public ResponseHeader Ping() {
            throw new NotImplementedException();
        }

        public SolrSchema GetSchema(string schemaFileName) {
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

        public MFunc<string, QueryOptions, SolrQueryResults<T>> queryStringOptions;

        public SolrQueryResults<T> Query(string q, QueryOptions options) {
            return queryStringOptions.Invoke(q, options);
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

        public ResponseHeader Commit() {
            throw new NotImplementedException();
        }

        public ResponseHeader Rollback() {
            throw new NotImplementedException();
        }

        public ResponseHeader Optimize() {
            throw new NotImplementedException();
        }

        public ResponseHeader Add(T doc) {
            throw new NotImplementedException();
        }

        public MFunc<T, AddParameters, ResponseHeader> addDocParams;

        public ResponseHeader Add(T doc, AddParameters parameters) {
            return addDocParams.Invoke(doc, parameters);
        }

        public ResponseHeader AddWithBoost(T doc, double boost) {
            throw new NotImplementedException();
        }

        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters) {
            throw new NotImplementedException();
        }

        public ExtractResponse Extract(ExtractParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader Add(IEnumerable<T> docs) {
            throw new NotImplementedException();
        }

        public ResponseHeader AddRange(IEnumerable<T> docs) {
            throw new NotImplementedException();
        }

        public ResponseHeader Add(IEnumerable<T> docs, AddParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            throw new NotImplementedException();
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            throw new NotImplementedException();
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(T doc) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(T doc, DeleteParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(IEnumerable<T> docs) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(ISolrQuery q) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(string id) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(string id, DeleteParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(IEnumerable<string> ids) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q) {
            throw new NotImplementedException();
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters) {
            throw new NotImplementedException();
        }

        public ResponseHeader BuildSpellCheckDictionary() {
            throw new NotImplementedException();
        }

        public IEnumerable<ValidationResult> EnumerateValidationResults() {
            throw new NotImplementedException();
        }
    }
}