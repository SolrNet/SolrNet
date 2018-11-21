using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrNet.Cloud
{
    public class SolrCloudOperations<T> : SolrCloudOperationsBase<T>, ISolrOperations<T>
    {
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false) { }
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection) { }

        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false, collectionName: collectionName) { }
        public SolrCloudOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection, collectionName) { }

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options)
        {
            return PerformOperation(operations => operations.Query(query, options));
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            return PerformOperation(operations => operations.MoreLikeThis(query, options));
        }

        public ResponseHeader Ping()
        {
            return PerformOperation(operations => operations.Ping());
        }

        public SolrSchema GetSchema(string schemaFileName)
        {
            return PerformOperation(operations => operations.GetSchema(schemaFileName));
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            return PerformOperation(operations => operations.GetDIHStatus(options));
        }

        public SolrQueryResults<T> Query(string q)
        {
            return PerformOperation(operations => operations.Query(q));
        }

        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders)
        {
            return PerformOperation(operations => operations.Query(q, orders));
        }

        public SolrQueryResults<T> Query(string q, QueryOptions options)
        {
            return PerformOperation(operations => operations.Query(q, options));
        }

        public SolrQueryResults<T> Query(ISolrQuery q)
        {
            return PerformOperation(operations => operations.Query(q));
        }

        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders)
        {
            return PerformOperation(operations => operations.Query(query, orders));
        }

        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets)
        {
            return PerformOperation(operations => operations.FacetFieldQuery(facets));
        }

        public ResponseHeader Commit()
        {
            return PerformOperation(operations => operations.Commit(), true);
        }

        public ResponseHeader Rollback()
        {
            return PerformOperation(operations => operations.Commit(), true);
        }

        public ResponseHeader Optimize()
        {
            return PerformOperation(operations => operations.Commit(), true);
        }

        public ResponseHeader Add(T doc)
        {
            return PerformOperation(operations => operations.Add(doc), true);
        }

        public ResponseHeader Add(T doc, AddParameters parameters)
        {
            return PerformOperation(operations => operations.Add(doc, parameters), true);
        }

        public ResponseHeader AddWithBoost(T doc, double boost)
        {
            return PerformOperation(operations => operations.AddWithBoost(doc, boost), true);
        }

        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters)
        {
            return PerformOperation(operations => operations.AddWithBoost(doc, boost, parameters), true);
        }

        public ExtractResponse Extract(ExtractParameters parameters)
        {
            return PerformOperation(operations => operations.Extract(parameters), true);
        }

        public ResponseHeader Add(IEnumerable<T> docs)
        {
            return PerformOperation(operations => operations.Add(docs), true);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs)
        {
            return PerformOperation(operations => operations.AddRange(docs), true);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters)
        {
            return PerformOperation(operations => operations.AddRange(docs, parameters), true);
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            return PerformOperation(operations => operations.AddWithBoost(docs), true);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            return PerformOperation(operations => operations.AddRangeWithBoost(docs), true);
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return PerformOperation(operations => operations.AddWithBoost(docs, parameters), true);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return PerformOperation(operations => operations.AddRangeWithBoost(docs, parameters), true);
        }

        public ResponseHeader Delete(T doc)
        {
            return PerformOperation(operations => operations.Delete(doc), true);
        }

        public ResponseHeader Delete(T doc, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(doc, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<T> docs)
        {
            return PerformOperation(operations => operations.Delete(docs), true);
        }

        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(docs, parameters), true);
        }

        public ResponseHeader Delete(ISolrQuery q)
        {
            return PerformOperation(operations => operations.Delete(q), true);
        }

        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(q, parameters), true);
        }

        public ResponseHeader Delete(string id)
        {
            return PerformOperation(operations => operations.Delete(id), true);
        }

        public ResponseHeader Delete(string id, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(id, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids)
        {
            return PerformOperation(operations => operations.Delete(ids), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(ids, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q)
        {
            return PerformOperation(operations => operations.Delete(ids, q), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(ids, q, parameters), true);
        }

        public ResponseHeader BuildSpellCheckDictionary()
        {
            return PerformOperation(operations => operations.BuildSpellCheckDictionary(), true);
        }

        public IEnumerable<ValidationResult> EnumerateValidationResults()
        {
            return PerformOperation(operations => operations.EnumerateValidationResults());
        }

        public ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            return PerformOperation(operations => operations.AtomicUpdate(doc, updateSpecs), true);
        }

        public ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            return PerformOperation(operations => operations.AtomicUpdate(id, updateSpecs), true);
        }

        public ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            return PerformOperation(operations => operations.AtomicUpdate(doc, updateSpecs, parameters), true);
        }

        public ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            return PerformOperation(operations => operations.AtomicUpdate(id, updateSpecs, parameters), true);
        }

        public Task<ResponseHeader> CommitAsync()
            => PerformOperation(operations => operations.CommitAsync());

        public Task<ResponseHeader> RollbackAsync()
            => PerformOperation(operations => operations.RollbackAsync());

        public Task<ResponseHeader> OptimizeAsync()
            => PerformOperation(operations => operations.OptimizeAsync());



        public Task<ResponseHeader> AddAsync(T doc)
            => PerformOperation(operations => operations.AddAsync(doc));

        public Task<ResponseHeader> AddAsync(T doc, AddParameters parameters)
            => PerformOperation(operations => operations.AddAsync(doc, parameters));



        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost)
            => PerformOperation(operations => operations.AddWithBoostAsync(doc, boost));



        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost, AddParameters parameters)
            => PerformOperation(operations => operations.AddWithBoostAsync(doc, boost, parameters));

        public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters)
            => PerformOperation(operations => operations.ExtractAsync(parameters));

        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs)
            => PerformOperation(operations => operations.AddRangeAsync(docs));

        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs, AddParameters parameters)
            => PerformOperation(operations => operations.AddRangeAsync(docs, parameters));

        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs)
            => PerformOperation(operations => operations.AddRangeWithBoostAsync(docs));

        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
            => PerformOperation(operations => operations.AddRangeWithBoostAsync(docs, parameters));

        public Task<ResponseHeader> DeleteAsync(T doc)
            => PerformOperation(operations => operations.DeleteAsync(doc));

        public Task<ResponseHeader> DeleteAsync(T doc, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(doc, parameters));

        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs)
            => PerformOperation(operations => operations.DeleteAsync(docs));

        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(docs, parameters));

        public Task<ResponseHeader> DeleteAsync(ISolrQuery q)
            => PerformOperation(operations => operations.DeleteAsync(q));

        public Task<ResponseHeader> DeleteAsync(ISolrQuery q, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(q, parameters));

        public Task<ResponseHeader> DeleteAsync(string id)
            => PerformOperation(operations => operations.DeleteAsync(id));

        public Task<ResponseHeader> DeleteAsync(string id, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(id, parameters));

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids)
            => PerformOperation(operations => operations.DeleteAsync(ids));

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(ids, parameters));

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q)
            => PerformOperation(operations => operations.DeleteAsync(ids, q));

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(ids, q, parameters));

        public Task<ResponseHeader> BuildSpellCheckDictionaryAsync()
            => PerformOperation(operations => operations.BuildSpellCheckDictionaryAsync());

        public Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync()
            => PerformOperation(operations => operations.EnumerateValidationResultsAsync());

        public Task<SolrQueryResults<T>> QueryAsync(string q, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(q, cancellationToken));

        public Task<SolrQueryResults<T>> QueryAsync(string q, ICollection<SortOrder> orders, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(q, orders, cancellationToken));

        public Task<SolrQueryResults<T>> QueryAsync(string q, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(q, options, cancellationToken));

        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery q, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(q, cancellationToken));

        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, ICollection<SortOrder> orders, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(query, orders, cancellationToken));

        public Task<ICollection<KeyValuePair<string, int>>> FacetFieldQueryAsync(SolrFacetFieldQuery facets)
            => PerformOperation(operations => operations.FacetFieldQueryAsync(facets));

        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(query, options, cancellationToken));

        public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.MoreLikeThisAsync(query, options));

        public Task<ResponseHeader> PingAsync()
            => PerformOperation(operations => operations.PingAsync());

        public Task<SolrSchema> GetSchemaAsync(string schemaFileName)
            => PerformOperation(operations => operations.GetSchemaAsync(schemaFileName));

        public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options)
            => PerformOperation(operations => operations.GetDIHStatusAsync(options));

        public Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs)
            => PerformOperation(operations => operations.AtomicUpdateAsync(doc, updateSpecs));

        public Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
            => PerformOperation(operations => operations.AtomicUpdateAsync(id, updateSpecs));

        public Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
            => PerformOperation(operations => operations.AtomicUpdateAsync(doc, updateSpecs, parameters));

        public Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
            => PerformOperation(operations => operations.AtomicUpdateAsync(id, updateSpecs, parameters));

        public IEnumerable<ValidationResult> EnumerateValidationResults(string schemaFileName)
        {
            return PerformOperation(operations => operations.EnumerateValidationResults(schemaFileName));
        }

        public Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync(string schemaFileName)
          => PerformOperation(operations => operations.EnumerateValidationResultsAsync(schemaFileName));
    }
}
