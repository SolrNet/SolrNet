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
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;

namespace SolrNet {
    /// <summary>
    /// Solr operations without convenience overloads
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public interface ISolrBasicOperations<T>: ISolrBasicReadOnlyOperations<T> {
        /// <summary>
        /// Commits posted documents
        /// </summary>
        /// <param name="options">Commit options</param>
        void Commit(WaitOptions options);

        /// <summary>
        /// Optimized Solr's index
        /// </summary>
        /// <param name="options">Optimization options</param>
        void Optimize(WaitOptions options);

        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <returns></returns>
        ISolrBasicOperations<T> Add(IEnumerable<T> docs);

        /// <summary>
        /// Deletes several documents (requires the document to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="ids">document ids to delete</param>
        /// <returns></returns>
        /// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ISolrBasicOperations<T> Delete(IEnumerable<string> ids);

        /// <summary>
        /// Deletes all documents that match a query
        /// </summary>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        ISolrBasicOperations<T> Delete(ISolrQuery q);

        /// <summary>
        /// Sends a custom command
        /// </summary>
        /// <param name="cmd">command to send</param>
        /// <returns>solr response</returns>
        string Send(ISolrCommand cmd);
    }
}