using System.Collections.Generic;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrNet.Cloud {
    public class SolrCloudOperations<T> : SolrCloudOperationsBase<T>, ISolrOperations<T> {
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false) { }
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection) { }

        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false, collectionName: collectionName) { }
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName, bool isPostConnection = false) 
            : base(cloudStateProvider, operationsProvider, isPostConnection, collectionName) { }

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return PerformOperation(operations => operations.Query(query, options));
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options) {
            return PerformOperation(operations => operations.MoreLikeThis(query, options));
        }

        public ResponseHeader Ping() {
            return PerformOperation(operations => operations.Ping());
        }

        public SolrSchema GetSchema(string schemaFileName) {
            return PerformOperation(operations => operations.GetSchema(schemaFileName));
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options) {
            return PerformOperation(operations => operations.GetDIHStatus(options));
        }

        public SolrQueryResults<T> Query(string q) {
            return PerformOperation(operations => operations.Query(q));
        }

        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders) {
            return PerformOperation(operations => operations.Query(q, orders));
        }

        public SolrQueryResults<T> Query(string q, QueryOptions options) {
            return PerformOperation(operations => operations.Query(q, options));
        }

        public SolrQueryResults<T> Query(ISolrQuery q) {
            return PerformOperation(operations => operations.Query(q));
        }

        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders) {
            return PerformOperation(operations => operations.Query(query, orders));
        }

        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets) {
            return PerformOperation(operations => operations.FacetFieldQuery(facets));
        }

        public ResponseHeader Commit() {
            return PerformOperation(operations => operations.Commit(), true);
        }

        public ResponseHeader Rollback() {
            return PerformOperation(operations => operations.Commit(), true);
        }

        public ResponseHeader Optimize() {
            return PerformOperation(operations => operations.Commit(), true);
        }

        public ResponseHeader Add(T doc) {
            return PerformOperation(operations => operations.Add(doc), true);
        }

        public ResponseHeader Add(T doc, AddParameters parameters) {
            return PerformOperation(operations => operations.Add(doc, parameters), true);
        }

        public ResponseHeader AddWithBoost(T doc, double boost) {
            return PerformOperation(operations => operations.AddWithBoost(doc, boost), true);
        }

        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters) {
            return PerformOperation(operations => operations.AddWithBoost(doc, boost, parameters), true);
        }

        public ExtractResponse Extract(ExtractParameters parameters) {
            return PerformOperation(operations => operations.Extract(parameters), true);
        }

        public ResponseHeader Add(IEnumerable<T> docs) {
            return PerformOperation(operations => operations.Add(docs), true);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs) {
            return PerformOperation(operations => operations.AddRange(docs), true);
        }

        public ResponseHeader Add(IEnumerable<T> docs, AddParameters parameters) {
            return PerformOperation(operations => operations.Add(docs, parameters), true);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters) {
            return PerformOperation(operations => operations.AddRange(docs, parameters), true);
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            return PerformOperation(operations => operations.AddWithBoost(docs), true);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            return PerformOperation(operations => operations.AddRangeWithBoost(docs), true);
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            return PerformOperation(operations => operations.AddWithBoost(docs, parameters), true);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            return PerformOperation(operations => operations.AddRangeWithBoost(docs, parameters), true);
        }

        public ResponseHeader Delete(T doc) {
            return PerformOperation(operations => operations.Delete(doc), true);
        }

        public ResponseHeader Delete(T doc, DeleteParameters parameters) {
            return PerformOperation(operations => operations.Delete(doc, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<T> docs) {
            return PerformOperation(operations => operations.Delete(docs), true);
        }

        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters) {
            return PerformOperation(operations => operations.Delete(docs, parameters), true);
        }

        public ResponseHeader Delete(ISolrQuery q) {
            return PerformOperation(operations => operations.Delete(q), true);
        }

        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters) {
            return PerformOperation(operations => operations.Delete(q, parameters), true);
        }

        public ResponseHeader Delete(string id) {
            return PerformOperation(operations => operations.Delete(id), true);
        }

        public ResponseHeader Delete(string id, DeleteParameters parameters) {
            return PerformOperation(operations => operations.Delete(id, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids) {
            return PerformOperation(operations => operations.Delete(ids), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters) {
            return PerformOperation(operations => operations.Delete(ids, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q) {
            return PerformOperation(operations => operations.Delete(ids, q), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters) {
            return PerformOperation(operations => operations.Delete(ids, q, parameters), true);
        }

        public ResponseHeader BuildSpellCheckDictionary() {
            return PerformOperation(operations => operations.BuildSpellCheckDictionary(), true);
        }

        public IEnumerable<ValidationResult> EnumerateValidationResults() {
            return PerformOperation(operations => operations.EnumerateValidationResults());
        }
    }
}