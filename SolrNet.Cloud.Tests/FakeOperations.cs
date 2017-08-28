using System.Collections.Generic;
using System.Threading.Tasks;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;
using System.Runtime.CompilerServices;

namespace SolrNet.Cloud.Tests
{
    public class FakeOperations<T> : ISolrBasicOperations<T>, ISolrOperations<T>
    {
        public FakeOperations(FakeProvider provider)
        {
            this.provider = provider;
        }

        private readonly FakeProvider provider;

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options)
        {
            SetLastOperation();
            return null;
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Ping()
        {
            SetLastOperation();
            return null;
        }

        public SolrSchema GetSchema(string schemaFileName)
        {
            SetLastOperation();
            return null;
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Commit(CommitOptions options)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Optimize(CommitOptions options)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Commit()
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Rollback()
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Optimize()
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Add(T doc)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Add(T doc, AddParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader AddWithBoost(T doc, double boost)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ExtractResponse Extract(ExtractParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Add(IEnumerable<T> docs)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader AddRange(IEnumerable<T> docs)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Add(IEnumerable<T> docs, AddParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(T doc)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(T doc, DeleteParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(IEnumerable<T> docs)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(ISolrQuery q)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(string id)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(string id, DeleteParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(IEnumerable<string> ids)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader BuildSpellCheckDictionary()
        {
            SetLastOperation();
            return null;
        }

        public IEnumerable<ValidationResult> EnumerateValidationResults()
        {
            SetLastOperation();
            return null;
        }

        public string Send(ISolrCommand cmd)
        {
            SetLastOperation();
            return null;
        }

        public ResponseHeader SendAndParseHeader(ISolrCommand cmd)
        {
            SetLastOperation();
            return null;
        }

        public ExtractResponse SendAndParseExtract(ISolrCommand cmd)
        {
            SetLastOperation();
            return null;
        }

        public SolrQueryResults<T> Query(string q)
        {
            SetLastOperation();
            return null;
        }

        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders)
        {
            SetLastOperation();
            return null;
        }

        public SolrQueryResults<T> Query(string q, QueryOptions options)
        {
            SetLastOperation();
            return null;
        }

        public SolrQueryResults<T> Query(ISolrQuery q)
        {
            SetLastOperation();
            return null;
        }

        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders)
        {
            SetLastOperation();
            return null;
        }

        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets)
        {
            SetLastOperation();
            return null;
        }

        public Task<ResponseHeader> CommitAsync(CommitOptions options)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> OptimizeAsync(CommitOptions options)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> RollbackAsync()
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<string> SendAsync(ISolrCommand cmd)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> SendAndParseHeaderAsync(ISolrCommand cmd)
        {
            SetLastOperation(); return null;
        }

        public Task<ExtractResponse> SendAndParseExtractAsync(ISolrCommand cmd)
        {
            SetLastOperation(); return null;
        }

        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options)
        {
            SetLastOperation(); return null;
        }

        public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> PingAsync()
        {
            SetLastOperation(); return null;
        }

        public Task<SolrSchema> GetSchemaAsync(string schemaFileName)
        {
            SetLastOperation(); return null;
        }

        public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> CommitAsync()
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> OptimizeAsync()
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddAsync(T doc)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddAsync(T doc, AddParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost, AddParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs, AddParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(T doc)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(T doc, DeleteParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs, DeleteParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(ISolrQuery q)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(ISolrQuery q, DeleteParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(string id)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(string id, DeleteParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, DeleteParameters parameters)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q)
        {
            SetLastOperation(); return null;
        }

        public Task<ResponseHeader> BuildSpellCheckDictionaryAsync()
        {
            SetLastOperation(); return null;
        }

        public Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync()
        {
            SetLastOperation(); return null;
        }

        public Task<SolrQueryResults<T>> QueryAsync(string q)
        {
            SetLastOperation(); return null;
        }

        public Task<SolrQueryResults<T>> QueryAsync(string q, ICollection<SortOrder> orders)
        {
            SetLastOperation(); return null;
        }

        public Task<SolrQueryResults<T>> QueryAsync(string q, QueryOptions options)
        {
            SetLastOperation(); return null;
        }

        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery q)
        {
            SetLastOperation(); return null;
        }

        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, ICollection<SortOrder> orders)
        {
            SetLastOperation(); return null;
        }

        public Task<ICollection<KeyValuePair<string, int>>> FacetFieldQueryAsync(SolrFacetFieldQuery facets)
        {
            SetLastOperation(); return null;
        }

        private void SetLastOperation([CallerMemberName]string operation = null)
        {
            provider.LastOperation = operation;

        }
    }
}
