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
        int NumFound { get; }

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
    }
}
