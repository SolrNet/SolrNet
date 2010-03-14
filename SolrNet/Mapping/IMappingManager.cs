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

using System.Reflection;

namespace SolrNet.Mapping {
    /// <summary>
    /// Mapping manager abstraction
    /// </summary>
    public interface IMappingManager : IReadOnlyMappingManager {
        /// <summary>
        /// Maps a property with the property name as Solr field name
        /// </summary>
        /// <param name="property">Document type property</param>
        void Add(PropertyInfo property);

        /// <summary>
        /// Maps a property
        /// </summary>
        /// <param name="property">Document type property</param>
        /// <param name="fieldName">Solr field name</param>
        void Add(PropertyInfo property, string fieldName);

        /// <summary>
        /// Maps a property
        /// </summary>
        /// <param name="property">Document type property</param>
        /// <param name="fieldName">Solr field name</param>
        /// <param name="boost">Index-time boosting</param>
        void Add(PropertyInfo property, string fieldName, float? boost);

        /// <summary>
        /// Sets unique key for a document type
        /// </summary>
        /// <param name="property">Document type property</param>
        void SetUniqueKey(PropertyInfo property);
    }
}