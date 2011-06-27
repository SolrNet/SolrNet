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

namespace SolrNet.Impl {
    public interface ISolrQueryByRange {
        string FieldName { get; }

        object From { get; }

        object To { get; }
        
        /// <summary>
        /// Is lower and upper bound inclusive
        /// </summary>
        bool Inclusive { get; }

        /// <summary>
        /// Is lower bound <see cref="From"/> inclusive
        /// ONLY available in Solr 4.0+
        /// </summary>
        bool InclusiveFrom { get; }

        /// <summary>
        /// Is upper bound <see cref="To"/> inclusive
        /// ONLY available in Solr 4.0+
        /// </summary>
        bool InclusiveTo { get; }
    }
}