using System;
using System.Collections.Generic;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;

namespace SolrNet {
    public class SolrServer<T> : ISolrOperations<T> where T : new() {
        private readonly ISolrBasicOperations<T> basicServer;
        private readonly IReadOnlyMappingManager mappingManager;

        public SolrServer(ISolrBasicOperations<T> basicServer, IReadOnlyMappingManager mappingManager) {
            this.basicServer = basicServer;
            this.mappingManager = mappingManager;
        }

        public ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return basicServer.Query(query, options);
        }

        public void Ping() {
            basicServer.Ping();
        }

        public ISolrQueryResults<T> Query(string q) {
            return Query(new SolrQuery(q));
        }

        public ISolrQueryResults<T> Query(string q, ICollection<SortOrder> orders) {
            return Query(new SolrQuery(q), orders);
        }

        public ISolrQueryResults<T> Query(string q, QueryOptions options) {
            return basicServer.Query(new SolrQuery(q), options);
        }

        public ISolrQueryResults<T> Query(ISolrQuery q) {
            return Query(q, new QueryOptions());
        }

        public ISolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders) {
            return Query(query, new QueryOptions { OrderBy = orders });
        }

        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facet) {
            var r = basicServer.Query(SolrQuery.All, new QueryOptions {
                Rows = 0,
                FacetQueries = new[] {facet},
            });
            return r.FacetFields[facet.Field];
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

        public ISolrOperations<T> Add(IEnumerable<T> docs) {
            basicServer.Add(docs);
            return this;
        }

        ISolrBasicOperations<T> ISolrBasicOperations<T>.Delete(string id) {
            return Delete(id);
        }

        public ISolrOperations<T> Delete(T doc) {
            var id = GetId(doc);
            Delete(id.ToString());
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
            basicServer.Delete(q);
            return this;
        }

        public ISolrOperations<T> Delete(string id) {
            var delete = new DeleteCommand(new DeleteByIdParam(id));
            basicServer.Send(delete);
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

        ISolrBasicOperations<T> ISolrBasicOperations<T>.Add(IEnumerable<T> docs) {
            return basicServer.Add(docs);
        }

        ISolrBasicOperations<T> ISolrBasicOperations<T>.Delete(ISolrQuery q) {
            return basicServer.Delete(q);
        }

        public string Send(ISolrCommand cmd) {
            return basicServer.Send(cmd);
        }
    }
}