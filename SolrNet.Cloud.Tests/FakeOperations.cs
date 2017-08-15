using System.Collections.Generic;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrNet.Cloud.Tests
{
    public class FakeOperations<T> : ISolrBasicOperations<T>, ISolrOperations<T>
    {
        public FakeOperations(FakeProvider provider) {
            this.provider = provider;
        }

        private readonly FakeProvider provider;

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options)
        {
            provider.LastOperation = "Query";
            return null;
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            provider.LastOperation = "MoreLikeThis";
            return null;
        }

        public ResponseHeader Ping()
        {
            provider.LastOperation = "Ping";
            return null;
        }

        public SolrSchema GetSchema(string schemaFileName)
        {
            provider.LastOperation = "GetSchema";
            return null;
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            provider.LastOperation = "GetDIHStatus";
            return null;
        }

        public ResponseHeader Commit(CommitOptions options)
        {
            provider.LastOperation = "Commit";
            return null;
        }

        public ResponseHeader Optimize(CommitOptions options)
        {
            provider.LastOperation = "Optimize";
            return null;
        }

        public ResponseHeader Commit()
        {
            provider.LastOperation = "Commit";
            return null;
        }

        public ResponseHeader Rollback()
        {
            provider.LastOperation = "Rollback";
            return null;
        }

        public ResponseHeader Optimize()
        {
            provider.LastOperation = "Optimize";
            return null;
        }

        public ResponseHeader Add(T doc)
        {
            provider.LastOperation = "Add";
            return null;
        }

        public ResponseHeader Add(T doc, AddParameters parameters)
        {
            provider.LastOperation = "Add";
            return null;
        }

        public ResponseHeader AddWithBoost(T doc, double boost)
        {
            provider.LastOperation = "AddWithBoost";
            return null;
        }

        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters)
        {
            provider.LastOperation = "AddWithBoost";
            return null;
        }

        public ExtractResponse Extract(ExtractParameters parameters)
        {
            provider.LastOperation = "Extract";
            return null;
        }

        public ResponseHeader Add(IEnumerable<T> docs)
        {
            provider.LastOperation = "Add";
            return null;
        }

        public ResponseHeader AddRange(IEnumerable<T> docs)
        {
            provider.LastOperation = "AddRange";
            return null;
        }

        public ResponseHeader Add(IEnumerable<T> docs, AddParameters parameters)
        {
            provider.LastOperation = "Add";
            return null;
        }

        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters)
        {
            provider.LastOperation = "AddRange";
            return null;
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            provider.LastOperation = "AddWithBoost";
            return null;
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            provider.LastOperation = "AddRangeWithBoost";
            return null;
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            provider.LastOperation = "AddWithBoost";
            return null;
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            provider.LastOperation = "AddRangeWithBoost";
            return null;
        }

        public ResponseHeader Delete(T doc)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(T doc, DeleteParameters parameters)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(IEnumerable<T> docs)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(ISolrQuery q)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(string id)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(string id, DeleteParameters parameters)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(IEnumerable<string> ids)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            provider.LastOperation = "Delete";
            return null;
        }

        public ResponseHeader BuildSpellCheckDictionary()
        {
            provider.LastOperation = "BuildSpellCheckDictionary";
            return null;
        }

        public IEnumerable<ValidationResult> EnumerateValidationResults()
        {
            provider.LastOperation = "EnumerateValidationResults";
            return null;
        }

        public string Send(ISolrCommand cmd)
        {
            provider.LastOperation = "Send";
            return null;
        }

        public ResponseHeader SendAndParseHeader(ISolrCommand cmd)
        {
            provider.LastOperation = "SendAndParseHeader";
            return null;
        }

        public ExtractResponse SendAndParseExtract(ISolrCommand cmd)
        {
            provider.LastOperation = "SendAndParseExtract";
            return null;
        }

        public SolrQueryResults<T> Query(string q)
        {
            provider.LastOperation = "Query";
            return null;
        }

        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders)
        {
            provider.LastOperation = "Query";
            return null;
        }

        public SolrQueryResults<T> Query(string q, QueryOptions options)
        {
            provider.LastOperation = "Query";
            return null;
        }

        public SolrQueryResults<T> Query(ISolrQuery q)
        {
            provider.LastOperation = "Query";
            return null;
        }

        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders)
        {
            provider.LastOperation = "Query";
            return null;
        }

        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets)
        {
            provider.LastOperation = "FacetFieldQuery";
            return null;
        }
    }
}
