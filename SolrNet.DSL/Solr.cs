#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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

using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Rhino.Commons;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;

namespace SolrNet.DSL {
    /// <summary>
    /// Solr DSL Entry point
    /// </summary>
    public static class Solr {
        public static DeleteBy Delete {
            get { return new DeleteBy(Connection); }
        }

        public static readonly string SolrConnectionKey = "ISolrConnection";

        /// <summary>
        /// thread-local or webcontext-local connection
        /// </summary>
        /// <seealso cref="http://www.ayende.com/Blog/archive/7447.aspx"/>
        /// <seealso cref="http://rhino-tools.svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/LocalDataImpl/"/>
        public static ISolrConnection Connection {
            private get { return (ISolrConnection) Local.Data[SolrConnectionKey]; }
            set { Local.Data[SolrConnectionKey] = value; }
        }

        public static void Add<T>(T document) {
            Add<T>(new[] {document});
        }

        public static void Add<T>(IEnumerable<T> documents) {
            var cmd = new AddCommand<T>(documents, ServiceLocator.Current.GetInstance<ISolrDocumentSerializer<T>>());
            cmd.Execute(Connection);
        }

        public static ISolrQueryResults<T> Query<T>(string s, int start, int rows) where T : new() {
            var q = new SolrQueryExecuter<T>(Connection, ServiceLocator.Current.GetInstance<ISolrQueryResultParser<T>>(), ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>());
            return q.Execute(new SolrQuery(s), new QueryOptions {Start = start, Rows = rows});
        }

        public static ISolrQueryResults<T> Query<T>(string s) where T : new() {
            var q = new SolrQueryExecuter<T>(Connection, ServiceLocator.Current.GetInstance<ISolrQueryResultParser<T>>(), ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>());
            return q.Execute(new SolrQuery(s), null);
        }

        public static ISolrQueryResults<T> Query<T>(string s, SortOrder order) where T : new() {
            return Query<T>(s, new[] {order});
        }

        public static ISolrQueryResults<T> Query<T>(string s, ICollection<SortOrder> order) where T : new() {
            var q = new SolrQueryExecuter<T>(Connection, ServiceLocator.Current.GetInstance<ISolrQueryResultParser<T>>(), ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>());
            return q.Execute(new SolrQuery(s), new QueryOptions {OrderBy = order});
        }

        public static ISolrQueryResults<T> Query<T>(string s, SortOrder order, int start, int rows) where T : new() {
            return Query<T>(s, new[] {order}, start, rows);
        }

        public static ISolrQueryResults<T> Query<T>(string s, ICollection<SortOrder> order, int start, int rows) where T : new() {
            var q = new SolrQueryExecuter<T>(Connection, ServiceLocator.Current.GetInstance<ISolrQueryResultParser<T>>(), ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>());
            return q.Execute(new SolrQuery(s), new QueryOptions {
                OrderBy = order,
                Start = start,
                Rows = rows,
            });
        }

        public static ISolrQueryResults<T> Query<T>(ISolrQuery q) where T : new() {
            var queryExecuter = new SolrQueryExecuter<T>(Connection, ServiceLocator.Current.GetInstance<ISolrQueryResultParser<T>>(), ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>());
            return queryExecuter.Execute(q, null);
        }

        public static ISolrQueryResults<T> Query<T>(ISolrQuery q, int start, int rows) where T : new() {
            var queryExecuter = new SolrQueryExecuter<T>(Connection, ServiceLocator.Current.GetInstance<ISolrQueryResultParser<T>>(), ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>());
            return queryExecuter.Execute(q, new QueryOptions {Start = start, Rows = rows});
        }

        public static ISolrQueryResults<T> Query<T>(SolrQuery query, SortOrder order) where T : new() {
            return Query<T>(query, new[] {order});
        }

        public static ISolrQueryResults<T> Query<T>(SolrQuery query, ICollection<SortOrder> orders) where T : new() {
            var queryExecuter = new SolrQueryExecuter<T>(Connection, ServiceLocator.Current.GetInstance<ISolrQueryResultParser<T>>(), ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>());
            return queryExecuter.Execute(query, new QueryOptions { OrderBy = orders });
        }

        public static IDSLQuery<T> Query<T>() where T : new() {
            return new DSLQuery<T>(Connection);
        }

        public static void Commit() {
            var cmd = new CommitCommand();
            cmd.Execute(Connection);
        }

        public static void Commit(bool waitFlush, bool waitSearcher) {
            var cmd = new CommitCommand {WaitFlush = waitFlush, WaitSearcher = waitSearcher};
            cmd.Execute(Connection);
        }

        public static void Optimize() {
            var cmd = new OptimizeCommand();
            cmd.Execute(Connection);
        }

        public static void Optimize(bool waitFlush, bool waitSearcher) {
            var cmd = new OptimizeCommand {WaitFlush = waitFlush, WaitSearcher = waitSearcher};
            cmd.Execute(Connection);
        }
    }
}