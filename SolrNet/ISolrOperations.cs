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
using System.IO;
using SolrNet.Exceptions;
using SolrNet.Mapping.Validation;
using System.Threading.Tasks;

namespace SolrNet
{
    /// <summary>
    /// Consolidating interface, exposes all operations
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public interface ISolrOperations<T> : ISolrReadOnlyOperations<T>
    {
        /// <summary>
        /// Commits posted documents, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        ResponseHeader Commit();

        /// <summary>
        /// Commits posted documents, 
        /// blocking until index changes are flushed to disk and
        /// blocking until a new searcher is opened and registered as the main query searcher, making the changes visible.
        /// </summary>
        Task<ResponseHeader> CommitAsync();

        /// <summary>
        /// Rollbacks all add/deletes made to the index since the last commit.
        /// </summary>
        /// <returns></returns>
        ResponseHeader Rollback();

        /// <summary>
        /// Rollbacks all add/deletes made to the index since the last commit.
        /// </summary>
        /// <returns></returns>
        Task<ResponseHeader> RollbackAsync();

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        ResponseHeader Optimize();

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        Task<ResponseHeader> OptimizeAsync();

        /// <summary>
        /// Adds / updates a document
        /// </summary>
        /// <param name="doc">document to add/update</param>
        /// <returns></returns>
        ResponseHeader Add(T doc);

        /// <summary>
        /// Adds / updates a document
        /// </summary>
        /// <param name="doc">document to add/update</param>
        /// <returns></returns>
        Task<ResponseHeader> AddAsync(T doc);

        /// <summary>
        /// Adds / updates a document with parameters
        /// </summary>
        /// <param name="doc">The document to add/update.</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        ResponseHeader Add(T doc, AddParameters parameters);

        /// <summary>
        /// Adds / updates a document with parameters
        /// </summary>
        /// <param name="doc">The document to add/update.</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        Task<ResponseHeader> AddAsync(T doc, AddParameters parameters);

        /// <summary>
        /// Adds / updates a document with index-time boost
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="boost"></param>
        /// <returns></returns>
        ResponseHeader AddWithBoost(T doc, double boost);

        /// <summary>
        /// Adds / updates a document with index-time boost
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="boost"></param>
        /// <returns></returns>
        Task<ResponseHeader> AddWithBoostAsync(T doc, double boost);

        /// <summary>
        /// Adds / updates a document with index-time boost and add parameters
        /// </summary>
        /// <param name="doc">The document to add/update.</param>
        /// <param name="boost">The index-time boost to apply.</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters);

        /// <summary>
        /// Adds / updates a document with index-time boost and add parameters
        /// </summary>
        /// <param name="doc">The document to add/update.</param>
        /// <param name="boost">The index-time boost to apply.</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        Task<ResponseHeader> AddWithBoostAsync(T doc, double boost, AddParameters parameters);

        /// <summary>
        /// Adds / updates the extracted content of a richdocument
        /// </summary>
        /// <param name="parameters">The extracttion parameters</param>
        /// <returns></returns>
        ExtractResponse Extract(ExtractParameters parameters);

        /// <summary>
        /// Adds / updates the extracted content of a richdocument
        /// </summary>
        /// <param name="parameters">The extracttion parameters</param>
        /// <returns></returns>
        Task<ExtractResponse> ExtractAsync(ExtractParameters parameters);

        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <returns></returns>
        [Obsolete("Use AddRange instead")]
        ResponseHeader Add(IEnumerable<T> docs);

        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <returns></returns>
        ResponseHeader AddRange(IEnumerable<T> docs);

        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <returns></returns>
        Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs);


        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters);

        /// <summary>
        /// Adds / updates several documents at once
        /// </summary>
        /// <param name="docs">documents to add/update</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        Task<ResponseHeader> AddRangeAsync(IEnumerable<T> docs, AddParameters parameters);

        /// <summary>
        /// Adds / updates documents with index-time boost
        /// </summary>
        /// <param name="docs">List of docs / boost. If boost is null, no boost is applied</param>
        /// <returns></returns>
        [Obsolete("Use AddRangeWithBoost instead")]
        ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs);

        /// <summary>
        /// Adds / updates documents with index-time boost
        /// </summary>
        /// <param name="docs">List of docs / boost. If boost is null, no boost is applied</param>
        /// <returns></returns>
        ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs);

        /// <summary>
        /// Adds / updates documents with index-time boost
        /// </summary>
        /// <param name="docs">List of docs / boost. If boost is null, no boost is applied</param>
        /// <returns></returns>
        Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs);


        /// <summary>
        /// Adds / updates documents with index-time boost and add parameters
        /// </summary>
        /// <param name="docs">List of docs / boost. If boost is null, no boost is applied</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        [Obsolete("Use AddRangeWithBoost instead")]
        ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters);

        /// <summary>
        /// Adds / updates documents with index-time boost and add parameters
        /// </summary>
        /// <param name="docs">List of docs / boost. If boost is null, no boost is applied</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters);

        /// <summary>
        /// Adds / updates documents with index-time boost and add parameters
        /// </summary>
        /// <param name="docs">List of docs / boost. If boost is null, no boost is applied</param>
        /// <param name="parameters">The add parameters.</param>
        /// <returns></returns>
        Task<ResponseHeader> AddRangeWithBoostAsync(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters);

        /// <summary>
        /// Deletes a document (requires the document to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="doc">document to delete</param>
        /// <returns></returns>
        /// <exception cref="SolrNetException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ResponseHeader Delete(T doc);

        /// <summary>
        /// Deletes a document (requires the document to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="doc">document to delete</param>
        /// <returns></returns>
        /// <exception cref="SolrNetException">throws if document type doesn't have a unique key or document has null unique key</exception>
        Task<ResponseHeader> DeleteAsync(T doc);

        /// <summary>
        /// Deletes a document (requires the document to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="doc">document to delete</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        /// <exception cref="SolrNetException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ResponseHeader Delete(T doc, DeleteParameters parameters);

        /// <summary>
        /// Deletes a document (requires the document to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="doc">document to delete</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        /// <exception cref="SolrNetException">throws if document type doesn't have a unique key or document has null unique key</exception>
        Task<ResponseHeader> DeleteAsync(T doc, DeleteParameters parameters);

        /// <summary>
        /// Deletes several documents (requires the document type to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="docs"></param>
        /// <returns></returns>
        /// <exception cref="SolrNetException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ResponseHeader Delete(IEnumerable<T> docs);

        /// <summary>
        /// Deletes several documents (requires the document type to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="docs"></param>
        /// <returns></returns>
        /// <exception cref="SolrNetException">throws if document type doesn't have a unique key or document has null unique key</exception>
        Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs);

        /// <summary>
        /// Deletes several documents (requires the document type to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="docs"></param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        /// <exception cref="SolrNetException">throws if document type doesn't have a unique key or document has null unique key</exception>
        ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters);

        /// <summary>
        /// Deletes several documents (requires the document type to have a unique key defined with non-null value)
        /// </summary>
        /// <param name="docs"></param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        /// <exception cref="SolrNetException">throws if document type doesn't have a unique key or document has null unique key</exception>
        Task<ResponseHeader> DeleteAsync(IEnumerable<T> docs, DeleteParameters parameters);

        /// <summary>
        /// Deletes all documents that match a query
        /// </summary>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        ResponseHeader Delete(ISolrQuery q);

        /// <summary>
        /// Deletes all documents that match a query
        /// </summary>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(ISolrQuery q);

        /// <summary>
        /// Deletes all documents that match a query
        /// </summary>
        /// <param name="q">query to match</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters);

        /// <summary>
        /// Deletes all documents that match a query
        /// </summary>
        /// <param name="q">query to match</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(ISolrQuery q, DeleteParameters parameters);

        /// <summary>
        /// Deletes a document by its id (unique key)
        /// </summary>
        /// <param name="id">document key</param>
        /// <returns></returns>
        ResponseHeader Delete(string id);

        /// <summary>
        /// Deletes a document by its id (unique key)
        /// </summary>
        /// <param name="id">document key</param>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(string id);

        /// <summary>
        /// Deletes a document by its id (unique key)
        /// </summary>
        /// <param name="id">document key</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        ResponseHeader Delete(string id, DeleteParameters parameters);

        /// <summary>
        /// Deletes a document by its id (unique key)
        /// </summary>
        /// <param name="id">document key</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(string id, DeleteParameters parameters);

        /// <summary>
        /// Deletes several documents by their id (unique key)
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <returns></returns>
        ResponseHeader Delete(IEnumerable<string> ids);

        /// <summary>
        /// Deletes several documents by their id (unique key)
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids);

        /// <summary>
        /// Deletes several documents by their id (unique key)
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters);

        /// <summary>
        /// Deletes several documents by their id (unique key)
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, DeleteParameters parameters);

        /// <summary>
        /// Deletes all documents that match the given id's or the query
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q);

        /// <summary>
        /// Deletes all documents that match the given id's or the query
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <param name="q">query to match</param>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q);

        /// <summary>
        /// Deletes all documents that match the given id's or the query
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <param name="q">query to match</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters);

        /// <summary>
        /// Deletes all documents that match the given id's or the query
        /// </summary>
        /// <param name="ids">document unique keys</param>
        /// <param name="q">query to match</param>
        /// <param name="parameters">The delete parameters</param>
        /// <returns></returns>
        Task<ResponseHeader> DeleteAsync(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters);

        /// <summary>
        /// Create the dictionary for use by the SolrSpellChecker. 
        /// In typical applications, one needs to build the dictionary before using it. 
        /// However, it may not always be necessary as it is possible to setup the spellchecker with a dictionary that already exists.
        /// </summary>
        ResponseHeader BuildSpellCheckDictionary();

        /// <summary>
        /// Create the dictionary for use by the SolrSpellChecker. 
        /// In typical applications, one needs to build the dictionary before using it. 
        /// However, it may not always be necessary as it is possible to setup the spellchecker with a dictionary that already exists.
        /// </summary>
        Task<ResponseHeader> BuildSpellCheckDictionaryAsync();

        ///<summary>
        /// Validates the mapping of the type T against the Solr schema XML document.
        ///</summary>
        ///<returns>
        /// A collection of <see cref="ValidationResult"/> objects containing warnings and error found validating
        /// the type's mapping against the Solr schema if any.</returns>
        IEnumerable<ValidationResult> EnumerateValidationResults();

        /// <summary>
        /// Validates the mapping of the type T against the Solr schema XML document.
        /// </summary>
        /// <param name="schemaFileName"></param>
        /// <returns></returns>
        IEnumerable<ValidationResult> EnumerateValidationResults(String schemaFileName);

        ///<summary>
        /// Validates the mapping of the type T against the Solr schema XML document.
        ///</summary>
        ///<returns>
        /// A collection of <see cref="ValidationResult"/> objects containing warnings and error found validating
        /// the type's mapping against the Solr schema if any.</returns>
        Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync();
		
		/// <summary>
        /// Validates the mapping of the type T against the Solr schema XML document.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ValidationResult>> EnumerateValidationResultsAsync(String schemaFileName);

        /// <summary>
        /// Updates a document according to the supplied specification.
        /// </summary>
        /// <param name="doc">The document to update</param>
        /// <param name="updateSpecs">The specification that defines the update</param>
        /// <returns></returns>
        ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs);

        /// <summary>
        /// Updates a document according to the supplied specification (asynchronous).
        /// </summary>
        /// <param name="doc">The document to update</param>
        /// <param name="updateSpecs">The specification that defines the update</param>
        /// <returns></returns>
        Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs);

        /// <summary>
        /// Updates the document with the supplied ID according to the supplied specification.
        /// </summary>
        /// <param name="id">The ID of the document to update</param>
        /// <param name="updateSpecs">The specification that defines the update</param>
        /// <returns></returns>
        ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs);

        /// <summary>
        /// Updates the document with the supplied ID according to the supplied specification (asynchronous).
        /// </summary>
        /// <param name="id">The ID of the document to update</param>
        /// <param name="updateSpecs">The specification that defines the update</param>
        /// <returns></returns>
        Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs);

        /// <summary>
        /// Updates a document according to the supplied specification.
        /// </summary>
        /// <param name="doc">The document to update</param>
        /// <param name="updateSpecs">The specification that defines the update</param>
        /// <param name="parameters">The atomic update parameters</param>
        /// <returns></returns>
        ResponseHeader AtomicUpdate(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters);

        /// <summary>
        /// Updates a document according to the supplied specification (asynchronous).
        /// </summary>
        /// <param name="doc">The document to update</param>
        /// <param name="updateSpecs">The specification that defines the update</param>
        /// <param name="parameters">The atomic update parameters</param>
        /// <returns></returns>
        Task<ResponseHeader> AtomicUpdateAsync(T doc, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters);

        /// <summary>
        /// Updates the document with the supplied ID according to the supplied specification.
        /// </summary>
        /// <param name="id">The ID of the document to update</param>
        /// <param name="updateSpecs">The specification that defines the update</param>
        /// <param name="parameters">The atomic update parameters</param>
        /// <returns></returns>
        ResponseHeader AtomicUpdate(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters);

        /// <summary>
        /// Updates the document with the supplied ID according to the supplied specification (asynchronous).
        /// </summary>
        /// <param name="id">The ID of the document to update</param>
        /// <param name="updateSpecs">The specification that defines the update</param>
        /// <param name="parameters">The atomic update parameters</param>
        /// <returns></returns>
        Task<ResponseHeader> AtomicUpdateAsync(string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters);
    }
}
