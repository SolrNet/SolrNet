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
using SolrNet.Exceptions;

namespace SolrNet {
    /// <summary>
    /// Consolidating interface, exposes all operations
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public interface ISolrOperations<T> : ISolrReadOnlyOperations<T>, ISolrBasicOperations<T> {
        /// <summary>
        /// Commits posts, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        void Commit();

        void Optimize();

        /// <summary>
        /// Adds / updates a document
        /// </summary>
        /// <param name="doc">document to add/update</param>
        /// <returns></returns>
        ISolrOperations<T> Add(T doc);

        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <returns></returns>
        new ISolrOperations<T> Add(IEnumerable<T> docs);

        /// <summary>
        /// Deletes a document (requires the document to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="doc">document to delete</param>
        /// <returns></returns>
        /// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ISolrOperations<T> Delete(T doc);

        /// <summary>
        /// Deletes all documents that match a query
        /// </summary>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        new ISolrOperations<T> Delete(ISolrQuery q);

        /// <summary>
        /// Deletes a document by its id (unique key)
        /// </summary>
        /// <param name="id">document key</param>
        /// <returns></returns>
        new ISolrOperations<T> Delete(string id);
    }
}