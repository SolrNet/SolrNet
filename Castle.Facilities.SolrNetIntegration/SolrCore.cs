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
using SolrNet;

namespace Castle.Facilities.SolrNetIntegration {
    /// <summary>
    /// Represents a Solr core for configuration
    /// </summary>
    internal class SolrCore {
        public string Id { get; private set; }
        public Type DocumentType { get; private set; }
        public string Url { get; private set; }

        /// <summary>
        /// Creates a new Solr core for configuration
        /// </summary>
        /// <param name="id">Component name for <see cref="ISolrOperations{T}"/></param>
        /// <param name="documentType">Document type</param>
        /// <param name="url">Core url</param>
        public SolrCore(string id, Type documentType, string url) {
            Id = id;
            DocumentType = documentType;
            Url = url;
        }

        /// <summary>
        /// Creates a new Solr core for configuration
        /// </summary>
        /// <param name="documentType">Document type</param>
        /// <param name="url">Core url</param>
        public SolrCore(Type documentType, string url) : this(Guid.NewGuid().ToString(), documentType, url) { }
    }
}