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

using NHibernate.Criterion;

namespace NHibernate.SolrNet {
    /// <summary>
    /// NHibernate <see cref="IQuery"/> for SolrNet queries
    /// </summary>
    public interface INHSolrQuery : IQuery {
        /// <summary>
        /// Set the maximum number of rows to retrieve.
        /// </summary>
        /// <param name="maxResults">The maximum number of rows to retreive</param>
        /// <returns>this</returns>
        new INHSolrQuery SetMaxResults(int maxResults);

        /// <summary>
        /// Sets the first row to retrieve.
        /// </summary>
        /// <param name="firstResult">The first row to retreive.</param>
        /// <returns>this</returns>
        new INHSolrQuery SetFirstResult(int firstResult);

        /// <summary>
        /// Sets sort order
        /// </summary>
        /// <param name="o">Sort order</param>
        /// <returns>this</returns>
        INHSolrQuery SetSort(Order o);
    }
}