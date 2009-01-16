using System;
using System.Collections.Generic;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
    public class SolrServer<T> : ISolrOperations<T> where T : new() {
        private readonly ISolrBasicOperations<T> basicServer;
        public IReadOnlyMappingManager MappingManager { get; set; }

        protected SolrServer() {
            MappingManager = ReadOnlyMappingManagerFactory.Create();
        }

        public SolrServer(string serverURL): this() {
            basicServer = new SolrBasicServer<T>(serverURL);
        }

        public SolrServer(ISolrConnection connection) : this() {
            basicServer = new SolrBasicServer<T>(connection);
        }

        public SolrServer(ISolrBasicOperations<T> basicServer) : this() {
            this.basicServer = basicServer;
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

        public void Commit(WaitOptions options) {
            basicServer.Commit(options);
        }

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
            var prop = MappingManager.GetUniqueKey(typeof(T)).Key;
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