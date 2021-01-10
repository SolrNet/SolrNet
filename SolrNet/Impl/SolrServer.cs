#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrNet.Impl
{
    /// <summary>
    /// Main component to interact with Solr
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrServer<T> : ISolrOperations<T>
    {
        private readonly ISolrBasicOperations<T> basicServer;
        private readonly IReadOnlyMappingManager mappingManager;
        private readonly IMappingValidator _schemaMappingValidator;

        /// <summary>
        /// Main component to interact with Solr
        /// </summary>
        public SolrServer(ISolrBasicOperations<T> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidator _schemaMappingValidator)
        {
            this.basicServer = basicServer;
            this.mappingManager = mappingManager;
            this._schemaMappingValidator = _schemaMappingValidator;
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options)
        {
            return basicServer.Query(query, options);
        }

        /// <inheritdoc />
        public ResponseHeader Ping()
        {
            return basicServer.Ping();
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(string q)
        {
            return Query(new SolrQuery(q));
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders)
        {
            return Query(new SolrQuery(q), orders);
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(string q, QueryOptions options)
        {
            return basicServer.Query(new SolrQuery(q), options);
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(ISolrQuery q)
        {
            return Query(q, new QueryOptions());
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders)
        {
            return Query(query, new QueryOptions { OrderBy = orders });
        }

        /// <summary>
        /// Executes a facet field query only
        /// </summary>
        /// <param name="facet"></param>
        /// <returns></returns>
        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facet)
        {
            var r = basicServer.Query(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                Facet = new FacetParameters
                {
                    Queries = new[] { facet },
                },
            });
            return r.FacetFields[facet.Field];
        }

        /// <inheritdoc />
        public ResponseHeader BuildSpellCheckDictionary()
        {
            var r = basicServer.Query(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                SpellCheck = new SpellCheckingParameters { Build = true },
            });
            return r.Header;
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(T doc, double boost)
        {
            return AddWithBoost(doc, boost, null);
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters)
        {
            return ((ISolrOperations<T>)this).AddRangeWithBoost(new[] { new KeyValuePair<T, double?>(doc, boost) }, parameters);
        }

        /// <inheritdoc />
        public ExtractResponse Extract(ExtractParameters parameters)
        {
            return basicServer.Extract(parameters);
        }

        /// <inheritdoc />
        [Obsolete("Use AddRange instead")]
        public ResponseHeader Add(IEnumerable<T> docs)
        {
            return Add(docs, null);
        }

        /// <inheritdoc />
        public ResponseHeader AddRange(IEnumerable<T> docs)
        {
            return AddRange(docs, null);
        }

        [Obsolete("Use AddRange instead")]
        public ResponseHeader Add(IEnumerable<T> docs, AddParameters parameters)
        {
            return basicServer.AddWithBoost(docs.Select(d => new KeyValuePair<T, double?>(d, null)), parameters);
        }

        /// <inheritdoc />
        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters)
        {
            return basicServer.AddWithBoost(docs.Select(d => new KeyValuePair<T, double?>(d, null)), parameters);
        }

        [Obsolete("Use AddRangeWithBoost instead")]
        ResponseHeader ISolrOperations<T>.AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            return ((ISolrOperations<T>)this).AddWithBoost(docs, null);
        }

        /// <inheritdoc />
        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs)
        {
            return ((ISolrOperations<T>)this).AddRangeWithBoost(docs, null);
        }

        [Obsolete("Use AddRangeWithBoost instead")]
        ResponseHeader ISolrOperations<T>.AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return basicServer.AddWithBoost(docs, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            return basicServer.AddWithBoost(docs, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids)
        {
            return basicServer.Delete(ids, null, null);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters)
        {
            return basicServer.Delete(ids, null, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(T doc)
        {
            return Delete(doc, null);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(T doc, DeleteParameters parameters)
        {
            var id = GetId(doc);
            return Delete(id.ToString(), parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<T> docs)
        {
            return Delete(docs, null);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters)
        {
            return basicServer.Delete(docs.Select(d =>
            {
                var uniqueKey = mappingManager.GetUniqueKey(typeof(T));
                if (uniqueKey == null)
                    throw new SolrNetException(string.Format("This operation requires a unique key, but type '{0}' has no declared unique key", typeof(T)));
                return Convert.ToString(uniqueKey.Property.GetValue(d, null));
            }), null, parameters);
        }

        private object GetId(T doc)
        {
            var uniqueKey = mappingManager.GetUniqueKey(typeof(T));
            if (uniqueKey == null)
                throw new SolrNetException(string.Format("This operation requires a unique key, but type '{0}' has no declared unique key", typeof(T)));
            var prop = uniqueKey.Property;
            return prop.GetValue(doc, null);
        }

        ResponseHeader ISolrOperations<T>.Delete(ISolrQuery q)
        {
            return basicServer.Delete(null, q, null);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters)
        {
            return basicServer.Delete(null, q, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(string id)
        {
            return basicServer.Delete(new[] { id }, null, null);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(string id, DeleteParameters parameters)
        {
            return basicServer.Delete(new[] { id }, null, parameters);
        }

        ResponseHeader ISolrOperations<T>.Delete(IEnumerable<string> ids, ISolrQuery q)
        {
            return basicServer.Delete(ids, q, null);
        }

        ResponseHeader ISolrOperations<T>.Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            return basicServer.Delete(ids, q, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader Commit()
        {
            return basicServer.Commit(null);
        }

        /// <summary>
        /// Rollbacks all add/deletes made to the index since the last commit.
        /// </summary>
        /// <returns></returns>
        public ResponseHeader Rollback()
        {
            return basicServer.Rollback();
        }

        /// <summary>
        /// Commits posts, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        public ResponseHeader Optimize()
        {
            return basicServer.Optimize(null);
        }

        /// <inheritdoc />
        public ResponseHeader Add(T doc)
        {
            return Add(doc, null);
        }

        /// <inheritdoc />
        public ResponseHeader Add(T doc, AddParameters parameters)
        {
            return AddRange(new[] { doc }, parameters);
        }

        public SolrSchema GetSchema()
        {
            return basicServer.GetSchema("schema.xml");
        }

        /// <inheritdoc />
        public SolrSchema GetSchema(string schemaFileName)
        {
            return basicServer.GetSchema(schemaFileName);
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> EnumerateValidationResults()
        {
            return EnumerateValidationResults("schema.xml");
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> EnumerateValidationResults(String schemaFileName)
        {
            var schema = GetSchema(schemaFileName);
            return _schemaMappingValidator.EnumerateValidationResults(typeof(T), schema);
        }

        /// <summary>
        /// Gets the DIH Status.
        /// </summary>
        /// <param name="options">command options</param>
        /// <returns>A XmlDocument containing the DIH Status XML.</returns>
        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options)
        {
            return basicServer.GetDIHStatus(options);
        }

        /// <inheritdoc />
        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            return basicServer.MoreLikeThis(query, options);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            string uniqueKey = mappingManager.GetUniqueKey(doc.GetType()).FieldName;
            return basicServer.AtomicUpdate(uniqueKey, GetId(doc).ToString(), updateSpecs, null);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            string uniqueKey = mappingManager.GetUniqueKey(doc.GetType()).FieldName;
            return basicServer.AtomicUpdateAsync(uniqueKey, GetId(doc).ToString(), updateSpecs, null);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            string uniqueKey = mappingManager.GetUniqueKey(typeof(T)).FieldName;
            return basicServer.AtomicUpdate(uniqueKey, id, updateSpecs, null);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            string uniqueKey = mappingManager.GetUniqueKey(typeof(T)).FieldName;
            return basicServer.AtomicUpdateAsync(uniqueKey, id, updateSpecs, null);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            string uniqueKey = mappingManager.GetUniqueKey(doc.GetType()).FieldName;
            return basicServer.AtomicUpdate(uniqueKey, GetId(doc).ToString(), updateSpecs, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            string uniqueKey = mappingManager.GetUniqueKey(doc.GetType()).FieldName;
            return basicServer.AtomicUpdateAsync(uniqueKey, GetId(doc).ToString(), updateSpecs, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            string uniqueKey = mappingManager.GetUniqueKey(typeof(T)).FieldName;
            return basicServer.AtomicUpdate(uniqueKey, id, updateSpecs, parameters);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            string uniqueKey = mappingManager.GetUniqueKey(typeof(T)).FieldName;
            return basicServer.AtomicUpdateAsync(uniqueKey, id, updateSpecs, parameters);
        }

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, CancellationToken cancellationToken = default(CancellationToken)) => QueryAsync(new SolrQuery(q),cancellationToken);

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, ICollection<SortOrder> orders, CancellationToken cancellationToken = default(CancellationToken)) => QueryAsync(new SolrQuery(q), orders, cancellationToken);
        
        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(string q, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken)) => QueryAsync(new SolrQuery(q), options, cancellationToken);

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery q, CancellationToken cancellationToken = default(CancellationToken)) => QueryAsync(q, new QueryOptions(), cancellationToken);

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, ICollection<SortOrder> orders, CancellationToken cancellationToken = default(CancellationToken)) => QueryAsync(query, new QueryOptions() { OrderBy = orders }, cancellationToken);

        /// <inheritdoc />
        public async Task<ICollection<KeyValuePair<string, int>>> FacetFieldQueryAsync(SolrFacetFieldQuery facet)
        {
            var r = await basicServer.QueryAsync(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                Facet = new FacetParameters
                {
                    Queries = new[] { facet },
                },
            });
            return r.FacetFields[facet.Field];
        }

        /// <inheritdoc />
        public Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken)) => basicServer.QueryAsync(query, options, cancellationToken);

        /// <inheritdoc />
        public Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default(CancellationToken)) => basicServer.MoreLikeThisAsync(query, options, cancellationToken);

        /// <inheritdoc />
        public Task<ResponseHeader> PingAsync() => basicServer.PingAsync();

        /// <inheritdoc />
        public Task<SolrSchema> GetSchemaAsync(string schemaFileName) => basicServer.GetSchemaAsync(schemaFileName);

        /// <inheritdoc />
        public Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options) => basicServer.GetDIHStatusAsync(options);

        /// <inheritdoc />
        public Task<ResponseHeader> CommitAsync() => basicServer.CommitAsync(null);

        /// <inheritdoc />
        public Task<ResponseHeader> RollbackAsync() => basicServer.RollbackAsync();

        /// <inheritdoc />
        public Task<ResponseHeader> OptimizeAsync() => basicServer.OptimizeAsync(null);

        /// <inheritdoc />
        public Task<ResponseHeader> AddAsync(T doc) => AddAsync(doc, null);

        /// <inheritdoc />
        public Task<ResponseHeader> AddAsync(T doc, AddParameters parameters) => AddRangeAsync(new[] { doc }, parameters);

        /// <inheritdoc />
        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost) => AddWithBoostAsync(doc, boost, null);

        /// <inheritdoc />
        public Task<ResponseHeader> AddWithBoostAsync(T doc, double boost, AddParameters parameters) => AddRangeWithBoostAsync(new[] { new KeyValuePair<T, double?>(doc, boost) }, parameters);

        /// <inheritdoc />
        public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters) => basicServer.ExtractAsync(parameters);

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs) => AddRangeAsync(docs, null);

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs, AddParameters parameters) =>   basicServer.AddWithBoostAsync(docs.Select(d => new KeyValuePair<T, double?>(d, null)), parameters);

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs) => AddRangeWithBoostAsync(docs, null);

        /// <inheritdoc />
        public Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) => basicServer.AddWithBoostAsync(docs, parameters);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(T doc) => DeleteAsync(doc, null);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(T doc, DeleteParameters parameters) => DeleteAsync(GetId(doc).ToString(), parameters);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs) => DeleteAsync(docs, null);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs, DeleteParameters parameters) => basicServer.DeleteAsync(docs.Select(d => GetId(d).ToString()), null, parameters);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(ISolrQuery q) => DeleteAsync(q, null);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(ISolrQuery q, DeleteParameters parameters) => basicServer.DeleteAsync(null, q, parameters);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(string id) => DeleteAsync(id, null);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(string id, DeleteParameters parameters) => basicServer.DeleteAsync(new[] { id }, null, parameters);
        
        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids) => basicServer.DeleteAsync(ids, null,null);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, DeleteParameters parameters) => basicServer.DeleteAsync(ids, null, parameters);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q) => basicServer.DeleteAsync(ids, q, null);

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters) => basicServer.DeleteAsync(ids, q, parameters);

        /// <inheritdoc />
        public async Task<ResponseHeader> BuildSpellCheckDictionaryAsync()
        {
            var r = await basicServer.QueryAsync(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                SpellCheck = new SpellCheckingParameters { Build = true },
            });
            return r.Header;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync()
        {
            var schema = await basicServer.GetSchemaAsync("schema.xml");
            return _schemaMappingValidator.EnumerateValidationResults(typeof(T), schema);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync(String schemaFileName)
        {
            var schema = await basicServer.GetSchemaAsync(schemaFileName);
            return _schemaMappingValidator.EnumerateValidationResults(typeof(T), schema);
        }
    }
}
