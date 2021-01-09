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

using System.Collections.Generic;
using SolrNet.Commands.Parameters;
using System.Threading.Tasks;

namespace SolrNet {
    /// <summary>
    /// Solr operations without convenience overloads
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public interface ISolrBasicOperations<T> : ISolrBasicReadOnlyOperations<T> {
        /// <summary>
        /// Commits posted documents
        /// </summary>
        /// <param name="options">Commit options</param>
        ResponseHeader Commit(CommitOptions options);

        /// <summary>
        /// Commits posted documents
        /// </summary>
        /// <param name="options">Commit options</param>
        Task<ResponseHeader> CommitAsync(CommitOptions options);

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        /// <param name="options">Optimization options</param>
        ResponseHeader Optimize(CommitOptions options);

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        /// <param name="options">Optimization options</param>
        Task<ResponseHeader> OptimizeAsync(CommitOptions options);

        /// <summary>
        /// Rollbacks all add/deletes made to the index since the last commit.
        /// </summary>
        ResponseHeader Rollback();

        /// <summary>
        /// Rollbacks all add/deletes made to the index since the last commit.
        /// </summary>
        Task<ResponseHeader> RollbackAsync();

        /// <summary>
        /// Adds / updates several documents with index-time boost
        /// </summary>
        /// <param name="docs"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters);

        /// <summary>
        /// Adds / updates several documents with index-time boost
        /// </summary>
        /// <param name="docs"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
       Task<ResponseHeader> AddWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters);

        /// <summary>
        /// Adds / updates the extracted contents of a richdocument
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ExtractResponse Extract(ExtractParameters parameters);

        /// <summary>
        /// Adds / updates the extracted contents of a richdocument
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<ExtractResponse> ExtractAsync(ExtractParameters parameters);

        /// <summary>
        /// Deletes all documents that match the given id's or the query
        /// </summary>
        /// <param name="ids">document ids to delete</param>
        /// <param name="q">query to match</param>
        /// <param name="parameters"/>
        /// <returns></returns>
        ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters);

        /// <summary>
        /// Deletes all documents that match the given id's or the query
        /// </summary>
        /// <param name="ids">document ids to delete</param>
        /// <param name="q">query to match</param>
        /// <param name="parameters"/>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters);

        /// <summary>
        /// Sends a custom command
        /// </summary>
        /// <param name="cmd">command to send</param>
        /// <returns>solr response</returns>
        string Send(ISolrCommand cmd);

        /// <summary>
        /// Sends a custom command
        /// </summary>
        /// <param name="cmd">command to send</param>
        /// <returns>solr response</returns>
        Task<string> SendAsync(ISolrCommand cmd);

        /// <summary>
        /// Sends a custom command, returns parsed header from xml response
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        ResponseHeader SendAndParseHeader(ISolrCommand cmd);

        /// <summary>
        /// Sends a custom command, returns parsed header from xml response
        /// </summary>
        Task<ResponseHeader> SendAndParseHeaderAsync(ISolrCommand cmd);

        /// <summary>
        /// Sends a custom command, returns parsed extract response from xml response
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        ExtractResponse SendAndParseExtract(ISolrCommand cmd);

        /// <summary>
        /// Sends a custom command, returns parsed extract response from xml response
        /// </summary>
        Task<ExtractResponse> SendAndParseExtractAsync(ISolrCommand cmd);

        /// <summary>
        /// Updates the document with the provided ID
        /// </summary>
        /// <param name="uniqueKey">Name of the unique key field</param>
        /// <param name="id">ID of the document to update</param>
        /// <param name="updateSpecs">Specifications for the updates</param>
        /// <param name="parameters">The atomic update parameters</param>
        /// <returns></returns>
        ResponseHeader AtomicUpdate(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters);
        
        /// <summary>
        /// Updates the document with the provided ID (asynchronous)
        /// </summary>
        /// <param name="uniqueKey">Name of the unique key field</param>
        /// <param name="id">ID of the document to update</param>
        /// <param name="updateSpecs">Specifications for the updates</param>
        /// <param name="parameters">The atomic update parameters</param>
        /// <returns></returns>
        Task<ResponseHeader> AtomicUpdateAsync(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters);
    }
}
