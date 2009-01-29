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
using SolrNet.Commands;
using SolrNet.Commands.Parameters;

namespace SolrNet {
    public class SolrBasicServer<T> : ISolrBasicOperations<T> where T : new() {
        private readonly ISolrConnection connection;
        private readonly ISolrQueryExecuter<T> queryExecuter;
        private readonly ISolrDocumentSerializer<T> documentSerializer;

        public SolrBasicServer(ISolrConnection connection, ISolrQueryExecuter<T> queryExecuter, ISolrDocumentSerializer<T> documentSerializer) {
            this.connection = connection;
            this.queryExecuter = queryExecuter;
            this.documentSerializer = documentSerializer;
        }

        public void Commit(WaitOptions options) {
            options = options ?? new WaitOptions();
            var cmd = new CommitCommand {WaitFlush = options.WaitFlush, WaitSearcher = options.WaitSearcher};
            Send(cmd);
        }

        public void Optimize(WaitOptions options) {
            options = options ?? new WaitOptions();
            var cmd = new OptimizeCommand {WaitFlush = options.WaitFlush, WaitSearcher = options.WaitSearcher};
            Send(cmd);
        }

        public ISolrBasicOperations<T> Add(IEnumerable<T> docs) {
            var cmd = new AddCommand<T>(docs, documentSerializer);
            Send(cmd);
            return this;
        }

        public ISolrBasicOperations<T> Delete(string id) {
            var cmd = new DeleteCommand(new DeleteByIdParam(id));
            Send(cmd);
            return this;
        }

        public ISolrBasicOperations<T> Delete(ISolrQuery q) {
            var delete = new DeleteCommand(new DeleteByQueryParam(q));
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