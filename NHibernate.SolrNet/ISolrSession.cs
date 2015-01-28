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

using SolrNet;
using System;

namespace NHibernate.SolrNet {
    /// <summary>
    /// NHibernate <see cref="ISession"/> with SolrNet extensions for querying
    /// </summary>
    [Obsolete("Deprecated. Replace with your own integration.")]
    public interface ISolrSession: ISession {
        /// <summary>
        /// Creates a Solr query
        /// </summary>
        /// <param name="query">Solr query</param>
        /// <returns>query object</returns>
        INHSolrQuery CreateSolrQuery(string query);

        /// <summary>
        /// Creates a Solr query
        /// </summary>
        /// <param name="query">Solr query</param>
        /// <returns>query object</returns>
        INHSolrQuery CreateSolrQuery(ISolrQuery query);
    }
}