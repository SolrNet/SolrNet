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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Diagnostics;
using SolrNet.Schema;
using SolrNet.Utils;

namespace SolrNet.Impl {
    /// <summary>
    /// Implements the basic Solr operations
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrBasicServer<T> : LowLevelSolrServer,  ISolrBasicOperations<T> {
        private readonly ISolrConnection connection;
        private readonly ISolrQueryExecuter<T> queryExecuter;
        private readonly ISolrDocumentSerializer<T> documentSerializer;
        private readonly ISolrSchemaParser schemaParser;
        private readonly ISolrHeaderResponseParser headerParser;
        private readonly ISolrQuerySerializer querySerializer;
        private readonly ISolrDIHStatusParser dihStatusParser;
        private readonly ISolrExtractResponseParser extractResponseParser;

        public SolrBasicServer(ISolrConnection connection, ISolrQueryExecuter<T> queryExecuter, ISolrDocumentSerializer<T> documentSerializer, ISolrSchemaParser schemaParser, ISolrHeaderResponseParser headerParser, ISolrQuerySerializer querySerializer, ISolrDIHStatusParser dihStatusParser, ISolrExtractResponseParser extractResponseParser) 
            : base(connection,headerParser) {
            this.connection = connection;
            this.extractResponseParser = extractResponseParser;
            this.queryExecuter = queryExecuter;
            this.documentSerializer = documentSerializer;
            this.schemaParser = schemaParser;
            this.headerParser = headerParser;
            this.querySerializer = querySerializer;
            this.dihStatusParser = dihStatusParser;
        }

        /// <inheritdoc />
        public ResponseHeader Commit(CommitOptions options)
        {
            CommitCommand cmd = GetCommitCommand(options);
            return SendAndParseHeader(cmd);
        }

        private static CommitCommand GetCommitCommand(CommitOptions options)
        {
            options = options ?? new CommitOptions();
            var cmd = new CommitCommand
            {
                WaitFlush = options.WaitFlush,
                WaitSearcher = options.WaitSearcher,
                ExpungeDeletes = options.ExpungeDeletes,
                MaxSegments = options.MaxSegments,
            };
            return cmd;
        }

        /// <inheritdoc />
        public ResponseHeader Optimize(CommitOptions options)
        {
            OptimizeCommand cmd = GetOptimizeCommand( options);
            return SendAndParseHeader(cmd);
        }

        private static OptimizeCommand GetOptimizeCommand( CommitOptions options)
        {
            options = options ?? new CommitOptions();
            var cmd = new OptimizeCommand
            {
                WaitFlush = options.WaitFlush,
                WaitSearcher = options.WaitSearcher,
                ExpungeDeletes = options.ExpungeDeletes,
                MaxSegments = options.MaxSegments,
            };
            return cmd;
        }

        /// <inheritdoc />
        public ResponseHeader Rollback() {
            return SendAndParseHeader(new RollbackCommand());
        }

        /// <inheritdoc />
        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            var cmd = new AddCommand<T>(docs, documentSerializer, parameters);
            return SendAndParseHeader(cmd);
        }

        /// <inheritdoc />
        public ExtractResponse Extract(ExtractParameters parameters) {
            var cmd = new ExtractCommand(parameters);
            return SendAndParseExtract(cmd);
        }

        /// <inheritdoc />
        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            var delete = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, q, querySerializer), parameters);
            return SendAndParseHeader(delete);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q) {
            var delete = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, q, querySerializer), null);
            return SendAndParseHeader(delete);
        }

        /// <inheritdoc />
        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) 
        {
            using (DiagnosticsUtil.StartSolrActivity(query))
            {
                var results = queryExecuter.Execute(query, options);
                DiagnosticsUtil.EnrichCurrentActivity(results.Header);
                return results;
            }
        }

        /// <inheritdoc />
        public ExtractResponse SendAndParseExtract(ISolrCommand cmd)
        {
            using (DiagnosticsUtil.StartSolrActivity(cmd))
            {
                var r = Send(cmd);
                var xml = XDocument.Parse(r);
                var response = extractResponseParser.Parse(xml);
                DiagnosticsUtil.EnrichCurrentActivity(response.ResponseHeader);
                return response;    
            }
        }

        /// <inheritdoc />
        public async Task<ExtractResponse> SendAndParseExtractAsync(ISolrCommand cmd)
        {
            using (DiagnosticsUtil.StartSolrActivity(cmd))
            {
                var r = await SendAsync(cmd);
                var xml = XDocument.Parse(r);
                var response = extractResponseParser.Parse(xml);
                DiagnosticsUtil.EnrichCurrentActivity(response.ResponseHeader);
                return response;
            }
        }

        /// <inheritdoc />
        [Obsolete("Use AtomicUpdates instead.")]
        public ResponseHeader AtomicUpdate(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters){
            return AtomicUpdates(uniqueKey, new AtomicUpdateSpecCollection()
            {
                { id, updateSpecs }
            }, parameters);
        }

        /// <inheritdoc />
        [Obsolete("Use AtomicUpdatesAsync instead.")]
        public Task<ResponseHeader> AtomicUpdateAsync(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            return AtomicUpdatesAsync(uniqueKey, new AtomicUpdateSpecCollection()
            {
                { id, updateSpecs }
            }, parameters);
        }

        /// <inheritdoc />
        public ResponseHeader AtomicUpdates(string uniqueKey, AtomicUpdateSpecCollection updateSpecs, AtomicUpdateParameters parameters)
        {
            var atomicUpdate = new AtomicUpdatesCommand(uniqueKey, updateSpecs, parameters);
            return SendAndParseHeader(atomicUpdate);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AtomicUpdatesAsync(string uniqueKey, AtomicUpdateSpecCollection updateSpecs, AtomicUpdateParameters parameters)
        {
            var atomicUpdate = new AtomicUpdatesCommand(uniqueKey, updateSpecs, parameters);
            return SendAndParseHeaderAsync(atomicUpdate);
        }

        /// <inheritdoc />
        public ResponseHeader Ping() {
            return SendAndParseHeader(new PingCommand());
        }

        /// <inheritdoc />
        public SolrSchema GetSchema(string schemaFileName) 
        {
            using (DiagnosticsUtil.StartSolrActivity("schema"))
            {
                var schemaXml = connection.Get("/admin/file", new[] { new KeyValuePair<string, string>("file", schemaFileName) });
                var schema = XDocument.Parse(schemaXml);
                return schemaParser.Parse(schema);    
            }
        }

        /// <inheritdoc />
        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options) 
        {
            using (DiagnosticsUtil.StartSolrActivity("dataimport"))
            {
                var response = connection.Get("/dataimport", null);
                var dihstatus = XDocument.Parse(response);
                return dihStatusParser.Parse(dihstatus);    
            }
        }

        /// <inheritdoc />
        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options)
        {
            using (DiagnosticsUtil.StartSolrActivity(query))
            {
                var results = queryExecuter.Execute(query, options);
                DiagnosticsUtil.EnrichCurrentActivity(results.Header);
                return results;
            }
        }

        /// <inheritdoc />
        public async Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (DiagnosticsUtil.StartSolrActivity(query))
            {
                var results = await queryExecuter.ExecuteAsync(query, options, cancellationToken);
                DiagnosticsUtil.EnrichCurrentActivity(results.Header);
                return results;
            }
        }

        /// <inheritdoc />
        public async Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (DiagnosticsUtil.StartSolrActivity(query))
            {
                var results = await queryExecuter.ExecuteAsync(query, options, cancellationToken);
                DiagnosticsUtil.EnrichCurrentActivity(results.Header);
                return results;
            }
        }

        /// <inheritdoc />
        public Task<ResponseHeader> PingAsync()
        {
            return SendAndParseHeaderAsync(new PingCommand());
        }

        /// <inheritdoc />
        public async Task<SolrSchema> GetSchemaAsync(string schemaFileName)
        {
            string schemaXml = await connection.GetAsync("/admin/file", new[] { new KeyValuePair<string, string>("file", schemaFileName) });
            var schema = XDocument.Parse(schemaXml);
            return schemaParser.Parse(schema);
        }

        /// <inheritdoc />
        public async Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options)
        {
            var response = await connection.GetAsync("/dataimport", null);
            var dihstatus = XDocument.Parse(response);
            return dihStatusParser.Parse(dihstatus);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> CommitAsync(CommitOptions options)
        {
            var cmd = GetCommitCommand(options);
            return SendAndParseHeaderAsync(cmd);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> OptimizeAsync(CommitOptions options)
        {
            var cmd = GetOptimizeCommand(options);
            return SendAndParseHeaderAsync(cmd);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> RollbackAsync()
        {
            return SendAndParseHeaderAsync(new RollbackCommand());
        }

        /// <inheritdoc />
        public Task<ResponseHeader> AddWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters)
        {
            var cmd = new AddCommand<T>(docs, documentSerializer, parameters);
            return SendAndParseHeaderAsync(cmd);
        }

        /// <inheritdoc />
        public Task<ExtractResponse> ExtractAsync(ExtractParameters parameters)
        {
            var cmd = new ExtractCommand(parameters);
            return SendAndParseExtractAsync(cmd);
        }

        /// <inheritdoc />
        public Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters)
        {
            var delete = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, q, querySerializer), parameters);
            return SendAndParseHeaderAsync(delete);
        }
    }

    public class LowLevelSolrServer
    {
        protected readonly ISolrHeaderResponseParser headerParser;
        protected readonly ISolrConnection connection;

        public LowLevelSolrServer(ISolrConnection connection, ISolrHeaderResponseParser parser)
        {
            this.headerParser = parser ?? new ResponseParsers.HeaderResponseParser();
            this.connection = connection;
        }
        
        public XDocument Send(string handler, IEnumerable<KeyValuePair<string, string>> solrParams)
        {
            using (DiagnosticsUtil.StartSolrActivity(solrParams))
            {
                var r = SendRaw(handler, solrParams);
                return XDocument.Parse(r);    
            }
        }

        public string Send(ISolrCommand cmd)
        {
            return cmd.Execute(connection);
        }

        public Task<string> SendAsync(ISolrCommand cmd)
        {
            return cmd.ExecuteAsync(connection);
        }

        public ResponseHeader SendAndParseHeader(string handler, IEnumerable<KeyValuePair<string, string>> solrParams)
        {
            using (DiagnosticsUtil.StartSolrActivity(solrParams))
            {
                var r = connection.Get(handler, solrParams);
                var xml = XDocument.Parse(r);
                var headers = headerParser.Parse(xml);
                DiagnosticsUtil.EnrichCurrentActivity(headers);
                return headers;
            }
        }

        public ResponseHeader SendAndParseHeader(ISolrCommand cmd)
        {
            using (DiagnosticsUtil.StartSolrActivity(cmd))
            {
                var r = Send(cmd);
                var xml = XDocument.Parse(r);
                var headers = headerParser.Parse(xml);
                DiagnosticsUtil.EnrichCurrentActivity(headers);
                return headers;
            }
        }

        public async Task<ResponseHeader> SendAndParseHeaderAsync(ISolrCommand cmd)
        {
            using (DiagnosticsUtil.StartSolrActivity(cmd))
            {
                var r = await SendAsync(cmd);
                var xml = XDocument.Parse(r);
                var headers = headerParser.Parse(xml);
                DiagnosticsUtil.EnrichCurrentActivity(headers);
                return headers;
            }
        }

        public string SendRaw(string handler, IEnumerable<KeyValuePair<string, string>> solrParams)
        {
            using (DiagnosticsUtil.StartSolrActivity(solrParams))
            {
                var r = connection.Get(handler, solrParams);
                return r;    
            }
        }
    }
}
