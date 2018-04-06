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
using SolrNet.Impl.FieldSerializers;

namespace SolrNet {


    /// <summary>
    /// Interval facet query
    /// <see href="https://lucene.apache.org/solr/guide/6_6/faceting.html#Faceting-IntervalFaceting"/>
    /// </summary>
    public class SolrFacetIntervalQuery : ISolrFacetQuery {

        /// <summary>
        /// Creates a date facet query
        /// </summary>
        /// <param name="field">Field to facet</param>
        public SolrFacetIntervalQuery(string field)
        {
            this.Field = field;
        }

        public string Field { get; }

        /// <summary>
        ///  The intervals for the field
        /// </summary>
        public ICollection<FacetIntervalSet> Sets { get; set; } = new List<FacetIntervalSet>();
     
    }
}
