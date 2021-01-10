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
    public class FacetDateInclude
    {
        protected readonly string value;

        protected FacetDateInclude(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// all gap based ranges include their lower bound
        /// </summary>
        public static FacetDateInclude Lower
        {
            get { return new FacetDateInclude("lower"); }
        }

        /// <summary>
        /// all gap based ranges include their upper bound
        /// </summary>
        public static FacetDateInclude Upper
        {
            get { return new FacetDateInclude("upper"); }
        }

        /// <summary>
        ///  the first and last gap ranges include their edge bounds (ie: lower for the first one, upper for the last one) 
        ///  even if the corresponding upper/lower option is not specified
        /// </summary>
        public static FacetDateInclude Edge
        {
            get { return new FacetDateInclude("edge"); }
        }

        /// <summary>
        /// the "before" and "after" ranges will be inclusive of their bounds, 
        /// even if the first or last ranges already include those boundaries.
        /// </summary>
        public static FacetDateInclude Outer
        {
            get { return new FacetDateInclude("outer"); }
        }

        /// <summary>
        /// shorthand for lower, upper, edge, outer
        /// </summary>
        public static FacetDateInclude All
        {
            get { return new FacetDateInclude("all"); }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var o = obj as FacetDateInclude;
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
