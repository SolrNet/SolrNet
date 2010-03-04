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
using SolrNet.Exceptions;
using SolrNet.Mapping.Validation;

namespace SolrNet {
    /// <summary>
    /// Consolidating interface, exposes all operations
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public interface ISolrOperations<T> : ISolrReadOnlyOperations<T>, ISolrBasicOperations<T> {
        /// <summary>
        /// Commits posted documents, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        void Commit();

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        void Optimize();

        /// <summary>
        /// Adds / updates a document
        /// </summary>
        /// <param name="doc">document to add/update</param>
        /// <returns></returns>
        ISolrOperations<T> Add(T doc);

        /// <summary>
        /// Adds / updates a document with index-time boost
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="boost"></param>
        /// <returns></returns>
        ISolrOperations<T> AddWithBoost(T doc, double boost);

        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <returns></returns>
        new ISolrOperations<T> Add(IEnumerable<T> docs);

        /// <summary>
        /// Adds / updates documents with index-time boost
        /// </summary>
        /// <param name="docs">List of docs / boost. If boost is null, no boost is applied</param>
        /// <returns></returns>
        new ISolrOperations<T> AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs);

        /// <summary>
        /// Deletes a document (requires the document to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="doc">document to delete</param>
        /// <returns></returns>
        /// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ISolrOperations<T> Delete(T doc);

        /// <summary>
        /// Deletes several documents (requires the document type to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="docs"></param>
        /// <returns></returns>
        /// <exception cref="NoUniqueKeyException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ISolrOperations<T> Delete(IEnumerable<T> docs);

        /// <summary>
        /// Deletes all documents that match a query
        /// </summary>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        ISolrOperations<T> Delete(ISolrQuery q);

        /// <summary>
        /// Deletes a document by its id (unique key)
        /// </summary>
        /// <param name="id">document key</param>
        /// <returns></returns>
        ISolrOperations<T> Delete(string id);

        /// <summary>
        /// Deletes several documents by their id (unique key)
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <returns></returns>
        ISolrOperations<T> Delete(IEnumerable<string> ids);

        /// <summary>
        /// Deletes all documents that match the given id's or the query
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        new ISolrOperations<T> Delete(IEnumerable<string> ids, ISolrQuery q);

        /// <summary>
        /// Create the dictionary for use by the SolrSpellChecker. 
        /// In typical applications, one needs to build the dictionary before using it. 
        /// However, it may not always be necessary as it is possible to setup the spellchecker with a dictionary that already exists.
        /// </summary>
        void BuildSpellCheckDictionary();

        ///<summary>
        /// Validates the mapping of the type T against the Solr schema XML document.
        ///</summary>
        /// <param name="validationRules">The validation rules to use.</param>
        ///<returns>
        /// A collection of <see cref="MappingValidationItem"/> objects containing warnings and error found validating
        /// the type's mapping against the Solr schema if any.</returns>
        ICollection<MappingValidationItem> Validate(IEnumerable<Type> validationRules);
    }
}