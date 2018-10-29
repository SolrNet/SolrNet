using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Schema;

namespace SolrNet.Cloud
{
    public class SolrCloudBasicOperations<T> : SolrCloudOperationsBase<T>, ISolrBasicOperations<T>
    {
        public SolrCloudBasicOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false) { }
        public SolrCloudBasicOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection) { }

        public SolrCloudBasicOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName)
            : base(cloudStateProvider, operationsProvider, isPostConnection: false, collectionName: collectionName) { }
        public SolrCloudBasicOperations(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName, bool isPostConnection = false)
            : base(cloudStateProvider, operationsProvider, isPostConnection, collectionName) { }

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options)
        {
            return PerformBasicOperation(operations => operations.Query(query, options));
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            return PerformBasicOperation(operations => operations.MoreLikeThis(query, options));
        }

        public ResponseHeader Ping()
        {
            return PerformBasicOperation(operations => operations.Ping());
        }

        public SolrSchema GetSchema(string schemaFileName)
        {
            return PerformBasicOperation(operations => operations.GetSchema(schemaFileName));
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            return PerformBasicOperation(operations => operations.GetDIHStatus(options));
        }

        public ResponseHeader Commit(CommitOptions options)
        {
            return PerformBasicOperation(operations => operations.Commit(options));
        }

        public ResponseHeader Optimize(CommitOptions options)
        {
            return PerformBasicOperation(operations => operations.Optimize(options));
        }

        public ResponseHeader Rollback()
        {
            return PerformBasicOperation(operations => operations.Rollback());
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return PerformBasicOperation(operations => operations.AddWithBoost(docs, parameters));
        }

        public ExtractResponse Extract(ExtractParameters parameters)
        {
            return PerformBasicOperation(operations => operations.Extract(parameters));
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            return PerformBasicOperation(operations => operations.Delete(ids, q, parameters));
        }

        public string Send(ISolrCommand cmd)
        {
            return PerformBasicOperation(operations => operations.Send(cmd));
        }

        public ResponseHeader SendAndParseHeader(ISolrCommand cmd)
        {
            return PerformBasicOperation(operations => operations.SendAndParseHeader(cmd));
        }

        public ExtractResponse SendAndParseExtract(ISolrCommand cmd)
        {
            return PerformBasicOperation(operations => operations.SendAndParseExtract(cmd));
        }

        public ResponseHeader AtomicUpdate(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            return PerformBasicOperation(operations => operations.AtomicUpdate(uniqueKey, id, updateSpecs, parameters));
        }

        public Task<ResponseHeader> AtomicUpdateAsync(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            return PerformBasicOperation(operations => operations.AtomicUpdateAsync(uniqueKey, id, updateSpecs, parameters));
        }

        public Task<ResponseHeader> CommitAsync(CommitOptions options)
        {
            return PerformBasicOperation(operations => operations.CommitAsync(options));
        }

        public Task<ResponseHeader> OptimizeAsync(CommitOptions options)
            => PerformBasicOperation(operations => operations.OptimizeAsync(options));

        public Task<ResponseHeader> RollbackAsync()
            => PerformBasicOperation(operations => operations.RollbackAsync());

        public Task<ResponseHeader> AddWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
            => PerformBasicOperation(operations => operations.AddWithBoostAsync(docs, parameters));

        public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters)
            => PerformBasicOperation(operations => operations.ExtractAsync(parameters));

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
            => PerformBasicOperation(operations => operations.DeleteAsync(ids, q, parameters));

        public Task<string> SendAsync(ISolrCommand cmd) 
            => PerformBasicOperation(operations => operations.SendAsync(cmd));

        public Task<ResponseHeader> SendAndParseHeaderAsync(ISolrCommand cmd)
            => PerformBasicOperation(operations => operations.SendAndParseHeaderAsync(cmd));

        public Task<ExtractResponse> SendAndParseExtractAsync(ISolrCommand cmd)
            => PerformBasicOperation(operations => operations.SendAndParseExtractAsync(cmd));

        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
            => PerformBasicOperation(operations => operations.QueryAsync(query,options,cancellationToken));

        public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
            => PerformBasicOperation(operations => operations.MoreLikeThisAsync(query,options, cancellationToken));

        public Task<ResponseHeader> PingAsync()
            => PerformBasicOperation(operations => operations.PingAsync());

        public Task<SolrSchema> GetSchemaAsync(string schemaFileName)
            => PerformBasicOperation(operations => operations.GetSchemaAsync(schemaFileName));

        public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options)
            => PerformBasicOperation(operations => operations.GetDIHStatusAsync(options));
    }
}
