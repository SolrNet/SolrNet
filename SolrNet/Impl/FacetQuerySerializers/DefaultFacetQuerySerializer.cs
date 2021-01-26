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

namespace SolrNet.Impl.FacetQuerySerializers {
    /// <summary>
    /// Default facet query serializer
    /// </summary>
    public class DefaultFacetQuerySerializer : ISolrFacetQuerySerializer {
        private readonly AggregateFacetQuerySerializer serializer;

        /// <summary>
        /// Default facet query serializer
        /// </summary>
        public DefaultFacetQuerySerializer(ISolrQuerySerializer querySerializer, ISolrFieldSerializer fieldSerializer) {
            serializer = new AggregateFacetQuerySerializer(new ISolrFacetQuerySerializer[] {
                new SolrFacetQuerySerializer(querySerializer),
                new SolrFacetDateQuerySerializer(fieldSerializer),
                new SolrFacetRangeQuerySerializer(fieldSerializer),
                new SolrFacetIntervalQuerySerializer(fieldSerializer),
                new SolrFacetFieldQuerySerializer(),
				new SolrFacetPivotQuerySerializer()
            });
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t) {
            return serializer.CanHandleType(t);
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, string>> Serialize(object q) {
            return serializer.Serialize(q);
        }
    }
}
