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
using SolrNet.Commands.Parameters;
using SolrNet.Impl.FieldSerializers;

namespace SolrNet {
    /// <summary>
    /// Date facet query
    /// <see href="http://wiki.apache.org/solr/SimpleFacetParameters#Facet_by_Range"/>
    /// </summary>
    public class SolrFacetRangeQuery : ISolrFacetQuery {
        private readonly string field;
        private readonly int start;
        private readonly int end;
        private readonly int gap;

        /// <summary>
        /// Creates a range facet query
        /// </summary>
        /// <param name="field">Field to facet</param>
        /// <param name="start">The lower bound for the first range for all Range Faceting on this field</param>
        /// <param name="end">The minimum upper bound for the last range for all Range Faceting on this field</param>
        /// <param name="gap">
        /// The size of each date range expressed as an interval to be added to the lower bound.
        /// </param>
        public SolrFacetRangeQuery(string field, int start, int end, int gap)
        {
            this.field = field;
            this.start = start;
            this.end = end;
            this.gap = gap;
            Other = new List<FacetRangeOther>();
            Include = new List<FacetRangeInclude>();
        }

        /// <summary>
        /// What to do in the event that the gap does not divide evenly between start and end. 
        /// If this is true, the last range constraint will have an upper bound of end; 
        /// if false, the last range will have the smallest possible upper bound greater then end such that the range is exactly gap wide. 
        /// The default is false.
        /// </summary>
        public bool? HardEnd { get; set; }

        /// <summary>
        /// Indicates that in addition to the counts for each range constraint between start and end, counts should also be computed for other
        /// </summary>
        public ICollection<FacetRangeOther> Other { get; set; }

        /// <summary>
        /// By default, the ranges used to compute range faceting between facet.range.start and facet.range.end are all inclusive of both endpoints, while the the "before" and "after" ranges are not inclusive. This behavior can be modified by 
        /// the facet.date.include param, which can be any combination of the following options...
        /// </summary>
        public ICollection<FacetRangeInclude> Include { get; set; }
     

        public string Field {
            get { return field; }
        }

        public int Start
        {
            get { return start; }
        }

        public int End
        {
            get { return end; }
        }

        public int Gap
        {
            get { return gap; }
        }

        /// <summary>
        /// This param indicates the minimum counts for facet fields should be included in the response.
        /// The default value is 0.
        /// This value is set on a field basis and takes presedence 
        /// over <see cref="FacetParameters.MinCount"/>
        /// </summary>
        public int? MinCount { get; set; }
    }
}
