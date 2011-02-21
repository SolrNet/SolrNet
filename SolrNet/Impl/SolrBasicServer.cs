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
using System.Xml.Linq;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Schema;

namespace SolrNet.Impl {
    /// <summary>
    /// Implements the basic Solr operations
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrBasicServer<T> : ISolrBasicOperations<T> {
        private readonly ISolrConnection connection;
        private readonly ISolrQueryExecuter<T> queryExecuter;
        private readonly ISolrDocumentSerializer<T> documentSerializer;
        private readonly ISolrSchemaParser schemaParser;
        private readonly ISolrHeaderResponseParser headerParser;
        private readonly ISolrQuerySerializer querySerializer;
        private readonly ISolrDIHStatusParser dihStatusParser;
        private readonly ISolrExtractResponseParser extractResponseParser;

        public SolrBasicServer(ISolrConnection connection, ISolrQueryExecuter<T> queryExecuter, ISolrDocumentSerializer<T> documentSerializer, ISolrSchemaParser schemaParser, ISolrHeaderResponseParser headerParser, ISolrQuerySerializer querySerializer, ISolrDIHStatusParser dihStatusParser, ISolrExtractResponseParser extractResponseParser) {
            this.connection = connection;
            this.extractResponseParser = extractResponseParser;
            this.queryExecuter = queryExecuter;
            this.documentSerializer = documentSerializer;
            this.schemaParser = schemaParser;
            this.headerParser = headerParser;
            this.querySerializer = querySerializer;
            this.dihStatusParser = dihStatusParser;
        }

        public ResponseHeader Commit(CommitOptions options) {
            options = options ?? new CommitOptions();
            var cmd = new CommitCommand {
                WaitFlush = options.WaitFlush, 
                WaitSearcher = options.WaitSearcher,
                ExpungeDeletes = options.ExpungeDeletes,
                MaxSegments = options.MaxSegments,
            };
            return SendAndParseHeader(cmd);
        }

        public ResponseHeader Optimize(CommitOptions options) {
            options = options ?? new CommitOptions();
            var cmd = new OptimizeCommand {
                WaitFlush = options.WaitFlush, 
                WaitSearcher = options.WaitSearcher,
                ExpungeDeletes = options.ExpungeDeletes,
                MaxSegments = options.MaxSegments,
            };
            return SendAndParseHeader(cmd);
        }

        public ResponseHeader Rollback() {
            return SendAndParseHeader(new RollbackCommand());
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            var cmd = new AddCommand<T>(docs, documentSerializer, parameters);
            return SendAndParseHeader(cmd);
        }

        public ExtractResponse Extract(ExtractParameters parameters) {
            var cmd = new ExtractCommand(parameters);
            return SendAndParseExtract(cmd);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q) {
            var delete = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, q, querySerializer));
            return SendAndParseHeader(delete);
        }

        public ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return queryExecuter.Execute(query, options);
        }

        public string Send(ISolrCommand cmd) {
            return cmd.Execute(connection);
        }

        public ExtractResponse SendAndParseExtract(ISolrCommand cmd) {
            var r = Send(cmd);
            var xml = XDocument.Parse(r);
            return extractResponseParser.Parse(xml);
        }

        public ResponseHeader SendAndParseHeader(ISolrCommand cmd) {
            var r = Send(cmd);
            var xml = XDocument.Parse(r);
            return headerParser.Parse(xml);
        }

        public ResponseHeader Ping() {
            return SendAndParseHeader(new PingCommand());
        }

        public SolrSchema GetSchema() {
            string schemaXml = new GetSchemaCommand().Execute(connection);
            var schema = XDocument.Parse(schemaXml);
            return schemaParser.Parse(schema);
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options) {
            var response = connection.Get("/dataimport", null);
            var dihstatus = XDocument.Parse(response);
            return dihStatusParser.Parse(dihstatus);
        }
    }
}
