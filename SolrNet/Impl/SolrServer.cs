﻿#region license
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
    public class SolrServer<T> : ISolrOperations<T> {
        private readonly ISolrBasicOperations<T> basicServer;
        private readonly IReadOnlyMappingManager mappingManager;
        private readonly IMappingValidator _schemaMappingValidator;

        public SolrServer(ISolrBasicOperations<T> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidator _schemaMappingValidator) {
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
        public ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return basicServer.Query(query, options);
        }

        public ResponseHeader Ping() {
            return basicServer.Ping();
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

        public ResponseHeader BuildSpellCheckDictionary() {
            var r = basicServer.Query(SolrQuery.All, new QueryOptions {
                Rows = 0,
                SpellCheck = new SpellCheckingParameters { Build = true },
            });
            return r.Header;
        }

        public ResponseHeader AddWithBoost(T doc, double boost) {
            return ((ISolrOperations<T>)this).AddWithBoost(new[] { new KeyValuePair<T, double?>(doc, boost) });
        }

        public ResponseHeader Add(IEnumerable<T> docs) {
            return basicServer.AddWithBoost(Func.Select(docs, d => new KeyValuePair<T, double?>(d, null)));
        }

        ResponseHeader ISolrOperations<T>.AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            return basicServer.AddWithBoost(docs);
        }

        public ResponseHeader Delete(IEnumerable<string> ids) {
            return basicServer.Delete(ids, null);
        }

        public ResponseHeader Delete(T doc) {
            var id = GetId(doc);
            return Delete(id.ToString());
        }

        public ResponseHeader Delete(IEnumerable<T> docs) {
            return basicServer.Delete(Func.Select(docs, d => {
                var uniqueKey = mappingManager.GetUniqueKey(typeof (T));
                if (uniqueKey == null)
                    throw new SolrNetException(string.Format("This operation requires a unique key, but type '{0}' has no declared unique key", typeof(T)));
                return Convert.ToString(uniqueKey.Property.GetValue(d, null));
            }), null);
        }

        private object GetId(T doc) {
            var uniqueKey = mappingManager.GetUniqueKey(typeof(T));
            if (uniqueKey == null)
                throw new SolrNetException(string.Format("This operation requires a unique key, but type '{0}' has no declared unique key", typeof(T)));
            var prop = uniqueKey.Property;
            return prop.GetValue(doc, null);
        }

        ResponseHeader ISolrOperations<T>.Delete(ISolrQuery q) {
            return basicServer.Delete(null, q);
        }

        public ResponseHeader Delete(string id) {
            return basicServer.Delete(new[] {id}, null);
        }

        ResponseHeader ISolrOperations<T>.Delete(IEnumerable<string> ids, ISolrQuery q) {
            return basicServer.Delete(ids, q);
        }

        public ResponseHeader Commit() {
            return basicServer.Commit(null);
        }

        /// <summary>
        /// Commits posts, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        public ResponseHeader Optimize() {
            return basicServer.Optimize(null);
        }

        public ResponseHeader Add(T doc) {
            return Add(new[] { doc });
        }

        public SolrSchema GetSchema() {
            return basicServer.GetSchema();
        }

        public IEnumerable<ValidationResult> EnumerateValidationResults() {
            var schema = GetSchema();
            return _schemaMappingValidator.EnumerateValidationResults(typeof(T), schema);
        }
    }
}