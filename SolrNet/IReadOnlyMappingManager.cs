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

namespace SolrNet {
    /// <summary>
    /// Service interface for mapping Solr fields to object properties
    /// </summary>
    public interface IReadOnlyMappingManager {
        /// <summary>
        /// Gets fields mapped for this type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Empty collection if <paramref name="type"/> is not mapped</returns>
        IDictionary<string,SolrFieldModel> GetFields(Type type);

        /// <summary>
        /// Gets unique key for the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        SolrFieldModel GetUniqueKey(Type type);

        ICollection<Type> GetRegisteredTypes();
    }
}