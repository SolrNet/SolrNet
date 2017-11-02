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
    /// Range facet query
    /// <see href="https://lucene.apache.org/solr/guide/6_6/faceting.html#Faceting-RangeFaceting"/>
    /// </summary>
    public class SolrFacetRangeQuery : ISolrFacetQuery {

        /// <summary>
        /// Creates a date facet query
        /// </summary>
        /// <param name="field">Field to facet</param>
        /// <param name="start">The start parameter specifies the lower bound of the ranges</param>
        /// <param name="end">The facet.range.end specifies the upper bound of the ranges.</param>
        /// <param name="gap">
        /// The span of each range expressed as a value to be added to the lower bound. For date fields, this should be expressed using the DateMathParser syntax.
        /// <see href="https://lucene.apache.org/solr/6_6_0//solr-core/org/apache/solr/util/DateMathParser.html"/>
        /// </param>
        public SolrFacetRangeQuery(string field, DateTime start, DateTime end, string gap) : this(field, DateTimeFieldSerializer.SerializeDate(start), DateTimeFieldSerializer.SerializeDate(end) ,gap)
        {
        
        }

        /// <summary>
        /// Creates a date facet query
        /// </summary>
        /// <param name="field">Field to facet</param>
        /// <param name="start">The start parameter specifies the lower bound of the ranges</param>
        /// <param name="end">The facet.range.end specifies the upper bound of the ranges.</param>
        /// <param name="gap">
        /// The span of each range expressed as a value to be added to the lower bound.
        /// </param>
        public SolrFacetRangeQuery(string field, int start, int end, int gap) : this(field, start.ToString(), end.ToString(), gap.ToString())
        {

        }

        /// <summary>
        /// Creates a date facet query
        /// </summary>
        /// <param name="field">Field to facet</param>
        /// <param name="start">The start parameter specifies the lower bound of the ranges</param>
        /// <param name="end">The facet.range.end specifies the upper bound of the ranges.</param>
        /// <param name="gap">
        /// The span of each range expressed as a value to be added to the lower bound. For date fields, this should be expressed using the DateMathParser syntax.
        /// <see href="https://lucene.apache.org/solr/6_6_0//solr-core/org/apache/solr/util/DateMathParser.html"/>
        /// </param>
        public SolrFacetRangeQuery(string field, string start, string end, string gap) {
            this.Field = field;
            this.Start = start;
            this.End = end;
            this.Gap = gap;
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
        public ICollection<FacetRangeOther> Other { get; set; } = new List<FacetRangeOther>();

        /// <summary>
        /// By default, the ranges used to compute date faceting between facet.date.start and facet.date.end are all inclusive of both endpoints, while the the "before" and "after" ranges are not inclusive. This behavior can be modified by 
        /// the facet.date.include param, which can be any combination of the following options...
        /// </summary>
        public ICollection<FacetRangeInclude> Include { get; set; } = new List<FacetRangeInclude>();
     

        public string Field {
            get; private set;
        }

        public string Start {
            get; private set;
        }

        public string End {
            get; private set;
        }

        public string Gap {
            get; set;
        }

        /// <summary>
        /// Sets the type of algorithm or method Solr should use for range faceting. Both methods produce the same results, but performance may vary.
        /// </summary>
        public FacetRangeMethod Method { get; set; }
    }
}
