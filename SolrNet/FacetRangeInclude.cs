using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet
{
    /// <summary>
    /// By default, the ranges used to compute date faceting between facet.date.start and facet.date.end are all inclusive of both endpoints, 
    /// while the the "before" and "after" ranges are not inclusive. This behavior can be modified by the facet.date.include param, which can be any combination of the following options...
    /// </summary>
    public class FacetRangeInclude
    {
        protected readonly string value;

        protected FacetRangeInclude(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// all gap based ranges include their lower bound
        /// </summary>
        public static FacetRangeInclude Lower
        {
            get { return new FacetRangeInclude("lower"); }
        }

        /// <summary>
        /// all gap based ranges include their upper bound
        /// </summary>
        public static FacetRangeInclude Upper
        {
            get { return new FacetRangeInclude("upper"); }
        }

        /// <summary>
        ///  the first and last gap ranges include their edge bounds (ie: lower for the first one, upper for the last one) 
        ///  even if the corresponding upper/lower option is not specified
        /// </summary>
        public static FacetRangeInclude Edge
        {
            get { return new FacetRangeInclude("edge"); }
        }

        /// <summary>
        /// the "before" and "after" ranges will be inclusive of their bounds, 
        /// even if the first or last ranges already include those boundaries.
        /// </summary>
        public static FacetRangeInclude Outer
        {
            get { return new FacetRangeInclude("outer"); }
        }

        /// <summary>
        /// shorthand for lower, upper, edge, outer
        /// </summary>
        public static FacetRangeInclude All
        {
            get { return new FacetRangeInclude("all"); }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var o = obj as FacetRangeInclude;
            if (o == null)
                return false;
            return o.value == value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return value;
        }
    }
}
