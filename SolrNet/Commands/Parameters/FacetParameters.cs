#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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

using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    public class FacetParameters {
        /// <summary>
        /// Collection of facet queries
        /// </summary>
        public ICollection<ISolrFacetQuery> Queries { get; set; }


		/// <summary>
		/// This param allows you to specify names of fields (of type DateField) which should be treated as date facets.
		/// </summary>
		/// <value>The dates.</value>
		public ICollection<string> Dates { get; set; }

		/// <summary>
		/// The lower bound for the first date range for all Date Faceting on this field.
		/// This should be a single date expression which may use the [WWW] DateMathParser syntax.
		/// </summary>
		/// <value>The date start.</value>
		public string DateStart { get; set; }

		/// <summary>
		/// The minimum upper bound for the last date range for all Date Faceting on this field (see facet.date.hardend for an explanation of what the actual end value may be greater).
		/// This should be a single date expression which may use the [WWW] DateMathParser syntax. 
		/// </summary>
		/// <value>The date end.</value>
		public string DateEnd { get; set; }

		/// <summary>
		/// The size of each date range expressed as an interval to be added to the lower bound using the [WWW] DateMathParser syntax.
		/// </summary>
		/// <value>The date gap.</value>
		public string DateGap { get; set; }

		/// <summary>
		/// A Boolean parameter instructing Solr what to do in the event that facet.date.gap does not divide evenly between facet.date.start and facet.date.end.
		/// If this is true, the last date range constraint will have an upper bound of facet.date.end; if false,
		/// the last date range will have the smallest possible upper bound greater then facet.date.end such that the range is exactly facet.date.gap wide. 
		/// </summary>
		/// <value>The date hard end.</value>
		public bool? DateHardEnd { get; set; }

		/// <summary>
		/// This param indicates that in addition to the counts for each date range constraint between facet.date.start and facet.date.end, counts should also be computed for...
		/// *     before all records with field values lower then lower bound of the first range
		///	*     after all records with field values greater then the upper bound of the last range
		///	*     between all records with field values between the start and end bounds of all ranges
		///	*     none compute none of this information
		///	*     all shortcut for before, between, and after
		/// </summary>
		/// <value>The date other.</value>
		public string DateOther { get; set; }

        /// <summary>
        /// Limits the terms on which to facet to those starting with the given string prefix.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Set to true, this parameter indicates that constraints should be sorted by their count. 
        /// If false, facets will be in their natural index order (unicode). 
        /// For terms in the ascii range, this will be alphabetically sorted. 
        /// The default is true if Limit is greater than 0, false otherwise.
        /// </summary>
        public bool? Sort { get; set; }

        /// <summary>
        /// This param indicates the maximum number of constraint counts that should be returned for the facet fields. 
        /// A negative value means unlimited. 
        /// The default value is 100. 
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// This param indicates an offset into the list of constraints to allow paging. 
        /// The default value is 0. 
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// This param indicates the minimum counts for facet fields should be included in the response.
        /// The default value is 0.
        /// </summary>
        public int? MinCount { get; set; }

        /// <summary>
        /// Set to true this param indicates that in addition to the Term based constraints of a facet field, a count of all matching results which have no value for the field should be computed
        /// Default is false
        /// </summary>
        public bool? Missing { get; set; }

		

        /// <summary>
        /// This param indicates the minimum document frequency (number of documents matching a term) for which the filterCache should be used when determining the constraint count for that term. 
        /// This is only used during the term enumeration method of faceting (field type faceting on multi-valued or full-text fields).
        /// A value greater than zero will decrease memory usage of the filterCache, but increase the query time. 
        /// When faceting on a field with a very large number of terms, and you wish to decrease memory usage, try a low value of 25 to 50 first.
        /// The default value is 0, causing the filterCache to be used for all terms in the field.
        /// </summary>
        public int? EnumCacheMinDf { get; set; }

        public FacetParameters() {
            Queries = new List<ISolrFacetQuery>();
        }
    }
}