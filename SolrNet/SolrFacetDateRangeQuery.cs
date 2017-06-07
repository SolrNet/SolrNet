using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet
{
    public class SolrFacetDateRangeQuery : ISolrFacetQuery
    {
        private readonly string field;
        private readonly DateTime start;
        private readonly DateTime end;
        private readonly string gap;

        public SolrFacetDateRangeQuery(string field, DateTime start, DateTime end, string gap) {
            this.field = field;
            this.start = start;
            this.end = end;
            this.gap = gap;
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


        public string Field
        {
            get { return field; }
        }

        public DateTime Start
        {
            get { return start; }
        }

        public DateTime End
        {
            get { return end; }
        }

        public string Gap
        {
            get { return gap; }
        }

        public int? MinCount { get; set; }
    }
}
