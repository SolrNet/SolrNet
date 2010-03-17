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
using SolrNet.Commands;
using SolrNet.Commands.Parameters;

namespace SolrNet.Impl {
    /// <summary>
    /// Implements the basic Solr operations
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrBasicServer<T> : ISolrBasicOperations<T> where T : new() {
        private readonly ISolrConnection connection;
        private readonly ISolrQueryExecuter<T> queryExecuter;
        private readonly ISolrDocumentSerializer<T> documentSerializer;

        public SolrBasicServer(ISolrConnection connection, ISolrQueryExecuter<T> queryExecuter, ISolrDocumentSerializer<T> documentSerializer) {
            this.connection = connection;
            this.queryExecuter = queryExecuter;
            this.documentSerializer = documentSerializer;
        }

        public void Commit(CommitOptions options) {
            options = options ?? new CommitOptions();
            var cmd = new CommitCommand {
                WaitFlush = options.WaitFlush, 
                WaitSearcher = options.WaitSearcher,
                ExpungeDeletes = options.ExpungeDeletes,
                MaxSegments = options.MaxSegments,
            };
            Send(cmd);
        }

        public void Optimize(CommitOptions options) {
            options = options ?? new CommitOptions();
            var cmd = new OptimizeCommand {
                WaitFlush = options.WaitFlush, 
                WaitSearcher = options.WaitSearcher,
                ExpungeDeletes = options.ExpungeDeletes,
                MaxSegments = options.MaxSegments,
            };
            Send(cmd);
        }

        public void Rollback() {
            Send(new RollbackCommand());
        }

        public ISolrBasicOperations<T> AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            var cmd = new AddCommand<T>(docs, documentSerializer);
            Send(cmd);
            return this;
        }

         public ISolrBasicOperations<T> Delete(IEnumerable<string> ids, ISolrQuery q) {
            var delete = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, q));
            delete.Execute(connection);
            return this;
        }

        public ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return queryExecuter.Execute(query, options);
        }

        public string Send(ISolrCommand cmd) {
            return cmd.Execute(connection);
        }

        public void Ping() {
            new PingCommand().Execute(connection);
        }
    }
}