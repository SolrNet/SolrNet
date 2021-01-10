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

        /// <inheritdoc />
        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            return PerformOperation(operations => operations.MoreLikeThis(query, options));
        }

        /// <inheritdoc />
        public ResponseHeader Ping()
        {
            return PerformOperation(operations => operations.Ping());
        }

        /// <inheritdoc />
        public SolrSchema GetSchema(string schemaFileName)
        {
            return PerformOperation(operations => operations.GetSchema(schemaFileName));
        }

        /// <inheritdoc />
        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            return PerformOperation(operations => operations.GetDIHStatus(options));
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(string q)
        {
            return PerformOperation(operations => operations.Query(q));
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders)
        {
            return PerformOperation(operations => operations.Query(q, orders));
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(string q, QueryOptions options)
        {
            return PerformOperation(operations => operations.Query(q, options));
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(ISolrQuery q)
        {
            return PerformOperation(operations => operations.Query(q));
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders)
        {
            return PerformOperation(operations => operations.Query(query, orders));
        }

        /// <inheritdoc />
        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets)
        {
            return PerformOperation(operations => operations.FacetFieldQuery(facets));
        }

        /// <inheritdoc />
        public ResponseHeader Commit()
        {
            return PerformOperation(operations => operations.Commit(), true);
        }

        /// <inheritdoc />
        public ResponseHeader Rollback()
        {
            return PerformOperation(operations => operations.Commit(), true);
        }

        /// <inheritdoc />
        public ResponseHeader Optimize()
        {
            return PerformOperation(operations => operations.Commit(), true);
        }

        /// <inheritdoc />
        public ResponseHeader Add(T doc)
        {
            return PerformOperation(operations => operations.Add(doc), true);
        }

        /// <inheritdoc />
        public ResponseHeader Add(T doc, AddParameters parameters)
        {
            return PerformOperation(operations => operations.Add(doc, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(T doc, double boost)
        {
            return PerformOperation(operations => operations.AddWithBoost(doc, boost), true);
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters)
        {
            return PerformOperation(operations => operations.AddWithBoost(doc, boost, parameters), true);
        }

        /// <inheritdoc />
        public ExtractResponse Extract(ExtractParameters parameters)
        {
            return PerformOperation(operations => operations.Extract(parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader Add(IEnumerable<T> docs)
        {
            return PerformOperation(operations => operations.Add(docs), true);
        }

        /// <inheritdoc />
        public ResponseHeader AddRange(IEnumerable<T> docs)
        {
            return PerformOperation(operations => operations.AddRange(docs), true);
        }

        /// <inheritdoc />
        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters)
        {
            return PerformOperation(operations => operations.AddRange(docs, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            return PerformOperation(operations => operations.AddWithBoost(docs), true);
        }

        /// <inheritdoc />
        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            return PerformOperation(operations => operations.AddRangeWithBoost(docs), true);
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return PerformOperation(operations => operations.AddWithBoost(docs, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return PerformOperation(operations => operations.AddRangeWithBoost(docs, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(T doc)
        {
            return PerformOperation(operations => operations.Delete(doc), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(T doc, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(doc, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<T> docs)
        {
            return PerformOperation(operations => operations.Delete(docs), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(docs, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(ISolrQuery q)
        {
            return PerformOperation(operations => operations.Delete(q), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(q, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(string id)
        {
            return PerformOperation(operations => operations.Delete(id), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(string id, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(id, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids)
        {
            return PerformOperation(operations => operations.Delete(ids), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(ids, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q)
        {
            return PerformOperation(operations => operations.Delete(ids, q), true);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            return PerformOperation(operations => operations.Delete(ids, q, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader BuildSpellCheckDictionary()
        {
            return PerformOperation(operations => operations.BuildSpellCheckDictionary(), true);
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> EnumerateValidationResults()
        {
            return PerformOperation(operations => operations.EnumerateValidationResults());
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            return PerformOperation(operations => operations.AtomicUpdate(doc, updateSpecs), true);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            return PerformOperation(operations => operations.AtomicUpdate(id, updateSpecs), true);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            return PerformOperation(operations => operations.AtomicUpdate(doc, updateSpecs, parameters), true);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            return PerformOperation(operations => operations.AtomicUpdate(id, updateSpecs, parameters), true);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> CommitAsync()
            => PerformOperation(operations => operations.CommitAsync());

        /// <inheritdoc />
        public Task<ResponseHeader> RollbackAsync()
            => PerformOperation(operations => operations.RollbackAsync());

        /// <inheritdoc />
        public Task<ResponseHeader> OptimizeAsync()
            => PerformOperation(operations => operations.OptimizeAsync());


        /// <inheritdoc />
        public Task<ResponseHeader> AddAsync(T doc)
            => PerformOperation(operations => operations.AddAsync(doc));

        /// <inheritdoc />
        public Task<ResponseHeader> AddAsync(T doc, AddParameters parameters)
            => PerformOperation(operations => operations.AddAsync(doc, parameters));


        /// <inheritdoc />
        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost)
            => PerformOperation(operations => operations.AddWithBoostAsync(doc, boost));


        /// <inheritdoc />
        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost, AddParameters parameters)
            => PerformOperation(operations => operations.AddWithBoostAsync(doc, boost, parameters));

        /// <inheritdoc />
        public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters)
            => PerformOperation(operations => operations.ExtractAsync(parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs)
            => PerformOperation(operations => operations.AddRangeAsync(docs));

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs, AddParameters parameters)
            => PerformOperation(operations => operations.AddRangeAsync(docs, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs)
            => PerformOperation(operations => operations.AddRangeWithBoostAsync(docs));

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
            => PerformOperation(operations => operations.AddRangeWithBoostAsync(docs, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(T doc)
            => PerformOperation(operations => operations.DeleteAsync(doc));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(T doc, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(doc, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs)
            => PerformOperation(operations => operations.DeleteAsync(docs));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(docs, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(ISolrQuery q)
            => PerformOperation(operations => operations.DeleteAsync(q));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(ISolrQuery q, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(q, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(string id)
            => PerformOperation(operations => operations.DeleteAsync(id));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(string id, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(id, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids)
            => PerformOperation(operations => operations.DeleteAsync(ids));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(ids, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q)
            => PerformOperation(operations => operations.DeleteAsync(ids, q));

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
            => PerformOperation(operations => operations.DeleteAsync(ids, q, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> BuildSpellCheckDictionaryAsync()
            => PerformOperation(operations => operations.BuildSpellCheckDictionaryAsync());

        /// <inheritdoc />
        public Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync()
            => PerformOperation(operations => operations.EnumerateValidationResultsAsync());

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(q, cancellationToken));

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, ICollection<SortOrder> orders, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(q, orders, cancellationToken));

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(q, options, cancellationToken));

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery q, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(q, cancellationToken));

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, ICollection<SortOrder> orders, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(query, orders, cancellationToken));

        /// <inheritdoc />
        public Task<ICollection<KeyValuePair<string, int>>> FacetFieldQueryAsync(SolrFacetFieldQuery facets)
            => PerformOperation(operations => operations.FacetFieldQueryAsync(facets));

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.QueryAsync(query, options, cancellationToken));

        /// <inheritdoc />
        public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
            => PerformOperation(operations => operations.MoreLikeThisAsync(query, options));

        /// <inheritdoc />
        public Task<ResponseHeader> PingAsync()
            => PerformOperation(operations => operations.PingAsync());

        /// <inheritdoc />
        public Task<SolrSchema> GetSchemaAsync(string schemaFileName)
            => PerformOperation(operations => operations.GetSchemaAsync(schemaFileName));

        /// <inheritdoc />
        public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options)
            => PerformOperation(operations => operations.GetDIHStatusAsync(options));

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs)
            => PerformOperation(operations => operations.AtomicUpdateAsync(doc, updateSpecs));

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
            => PerformOperation(operations => operations.AtomicUpdateAsync(id, updateSpecs));

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
            => PerformOperation(operations => operations.AtomicUpdateAsync(doc, updateSpecs, parameters));

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
            => PerformOperation(operations => operations.AtomicUpdateAsync(id, updateSpecs, parameters));

        /// <inheritdoc />
        public IEnumerable<ValidationResult> EnumerateValidationResults(string schemaFileName)
        {
            return PerformOperation(operations => operations.EnumerateValidationResults(schemaFileName));
        }

        /// <inheritdoc />
        public Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync(string schemaFileName)
          => PerformOperation(operations => operations.EnumerateValidationResultsAsync(schemaFileName));
    }
}
