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
using System.Xml;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;
using SolrNet.Utils;

namespace SolrNet.Impl {
    /// <summary>
    /// Main component to interact with Solr
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrServer<T> : ISolrOperations<T> where T : new() {
        private readonly ISolrBasicOperations<T> basicServer;
        private readonly IReadOnlyMappingManager mappingManager;
        private readonly IMappingValidationManager schemaMappingValidationManager;
        private readonly ISolrSchemaParser schemaParser;

        public SolrServer(ISolrBasicOperations<T> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidationManager schemaMappingValidationManager, ISolrSchemaParser schemaParser) {
            this.basicServer = basicServer;
            this.mappingManager = mappingManager;
            this.schemaMappingValidationManager = schemaMappingValidationManager;
            this.schemaParser = schemaParser;
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return basicServer.Query(query, options);
        }

        public void Ping() {
            basicServer.Ping();
        }

        public ISolrQueryResults<T> Query(string q) {
            return Query(new SolrQuery(q));
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public ISolrQueryResults<T> Query(string q, ICollection<SortOrder> orders) {
            return Query(new SolrQuery(q), orders);
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ISolrQueryResults<T> Query(string q, QueryOptions options) {
            return basicServer.Query(new SolrQuery(q), options);
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public ISolrQueryResults<T> Query(ISolrQuery q) {
            return Query(q, new QueryOptions());
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public ISolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders) {
            return Query(query, new QueryOptions { OrderBy = orders });
        }

        /// <summary>
        /// Executes a facet field query only
        /// </summary>
        /// <param name="facet"></param>
        /// <returns></returns>
        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facet) {
            var r = basicServer.Query(SolrQuery.All, new QueryOptions {
                Rows = 0,
                Facet = new FacetParameters {
                    Queries = new[] {facet},
                },
            });
            return r.FacetFields[facet.Field];
        }

        public void BuildSpellCheckDictionary() {
            basicServer.Query(SolrQuery.All, new QueryOptions {
                Rows = 0,
                SpellCheck = new SpellCheckingParameters { Build = true },
            });
        }

        public void Commit(WaitOptions options) {
            basicServer.Commit(options);
        }

        /// <summary>
        /// Commits posts
        /// </summary>
        public void Optimize(WaitOptions options) {
            basicServer.Optimize(options);
        }

        public void Rollback() {
            basicServer.Rollback();
        }

        public ISolrOperations<T> AddWithBoost(T doc, double boost) {
            return ((ISolrOperations<T>)this).AddWithBoost(new[] { new KeyValuePair<T, double?>(doc, boost) });
        }

        public ISolrOperations<T> Add(IEnumerable<T> docs) {
            basicServer.Add(docs);
            return this;
        }

        ISolrOperations<T> ISolrOperations<T>.AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            basicServer.AddWithBoost(docs);
            return this;
        }

        public ISolrOperations<T> Delete(IEnumerable<string> ids) {
            basicServer.Delete(ids, null);
            return this;
        }

        public ISolrOperations<T> Delete(T doc) {
            var id = GetId(doc);
            Delete(id.ToString());
            return this;
        }

        public ISolrOperations<T> Delete(IEnumerable<T> docs) {
            basicServer.Delete(Func.Select(docs,
                                           d => Convert.ToString(mappingManager.GetUniqueKey(typeof (T)).Key.GetValue(d, null))), null);
            return this;
        }

        private object GetId(T doc) {
            var prop = mappingManager.GetUniqueKey(typeof(T)).Key;
            var id = prop.GetValue(doc, null);
            if (id == null)
                throw new NoUniqueKeyException(typeof(T));
            return id;
        }

        ISolrOperations<T> ISolrOperations<T>.Delete(ISolrQuery q) {
            basicServer.Delete(null, q);
            return this;
        }

        public ISolrOperations<T> Delete(string id) {
            var delete = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null));
            basicServer.Send(delete);
            return this;
        }

        ISolrOperations<T> ISolrOperations<T>.Delete(IEnumerable<string> ids, ISolrQuery q) {
            basicServer.Delete(ids, q);
            return this;
        }

        public void Commit() {
            basicServer.Commit(null);
        }

        /// <summary>
        /// Commits posts, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        public void Optimize() {
            basicServer.Optimize(null);
        }

        public ISolrOperations<T> Add(T doc) {
            Add(new[] { doc });
            return this;
        }

        public XmlDocument GetSchema() {
            return basicServer.GetSchema();
        }

        public IEnumerable<MappingValidationItem> Validate() {
            var schema = schemaParser.Parse(GetSchema());
            return schemaMappingValidationManager.Validate<T>(schema);
        }

        public string Send(ISolrCommand cmd) {
            return basicServer.Send(cmd);
        }
    }
}