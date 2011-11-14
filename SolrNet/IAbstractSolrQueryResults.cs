using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet
{
    public interface IAbstractSolrQueryResults<T> : IList<T>
    {
        /// <summary>
        /// Total documents found
        /// </summary>
        int NumFound { get; set; }

        /// <summary>
        /// Max score in these results
        /// </summary>
        double? MaxScore { get; set; }

        /// <summary>
        /// Query response header
        /// </summary>
        ResponseHeader Header { get; set; }

        /// <summary>
        /// Facet queries results
        /// </summary>
        IDictionary<string, int> FacetQueries { get; set; }

        /// <summary>
        /// Facet field queries results
        /// </summary>
        IDictionary<string, ICollection<KeyValuePair<string, int>>> FacetFields { get; set; }

        /// <summary>
        /// Date faceting results
        /// </summary>
        IDictionary<string, DateFacetingResult> FacetDates { get; set; }

        /// <summary>
        /// Pivot faceting results
        /// </summary>
        IDictionary<string, IList<Pivot>> FacetPivots { get; set; }
    }
}
