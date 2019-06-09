﻿#region license
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

namespace SolrNet {
    /// <summary>
    /// Date facet query
    /// <see href="http://wiki.apache.org/solr/SimpleFacetParameters#Date_Faceting_Parameters"/>
    /// </summary>
    [Obsolete("As of Solr 3.1 has been deprecated, as of Solr 6.6 unsupported.")]
    public class SolrFacetDateQuery : ISolrFacetQuery {
        private readonly string field;
        private readonly DateTime start;
        private readonly DateTime end;
        private readonly string gap;

        /// <summary>
        /// Creates a date facet query
        /// </summary>
        /// <param name="field">Field to facet</param>
        /// <param name="start">The lower bound for the first date range for all Date Faceting on this field</param>
        /// <param name="end">The minimum upper bound for the last date range for all Date Faceting on this field</param>
        /// <param name="gap">
        /// The size of each date range expressed as an interval to be added to the lower bound using the DateMathParser syntax.
        /// <see href="http://lucene.apache.org/solr/api/org/apache/solr/util/DateMathParser.html"/>
        /// </param>
        public SolrFacetDateQuery(string field, DateTime start, DateTime end, string gap) {
            this.field = field;
            this.start = start;
            this.end = end;
            this.gap = gap;
            Other = new List<FacetDateOther>();
            Include = new List<FacetDateInclude>();
        }

        /// <summary>
        /// What to do in the event that the gap does not divide evenly between start and end. 
        /// If this is true, the last date range constraint will have an upper bound of end; 
        /// if false, the last date range will have the smallest possible upper bound greater then end such that the range is exactly gap wide. 
        /// The default is false.
        /// </summary>
        public bool? HardEnd { get; set; }

        /// <summary>
        /// Indicates that in addition to the counts for each date range constraint between start and end, counts should also be computed for other
        /// </summary>
        public ICollection<FacetDateOther> Other { get; set; }
        
           /// <summary>
        /// By default, the ranges used to compute date faceting between facet.date.start and facet.date.end are all inclusive of both endpoints, while the the "before" and "after" ranges are not inclusive. This behavior can be modified by 
        /// the facet.date.include param, which can be any combination of the following options...
        /// </summary>
        public ICollection<FacetDateInclude> Include { get; set; }
     

        public string Field {
            get { return field; }
        }

        public DateTime Start {
            get { return start; }
        }

        public DateTime End {
            get { return end; }
        }

        public string Gap {
            get { return gap; }
        }
		
        /// <summary>
        /// This param indicates the minimum counts for facet fields should be included in the response.
        /// Solr wil default to 0 if not set
        /// This value is set on a field basis and takes presedence 
        /// over <see cref="FacetParameters.MinCount"/>
        /// </summary>
        public int? MinCount { get; set; }
    }
}
