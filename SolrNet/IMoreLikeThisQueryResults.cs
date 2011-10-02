using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet
{
    /// <summary>
    /// MoreLikeThisHandler query results
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMoreLikeThisQueryResults<T> : IAbstractSolrQueryResults<T>
    {
        /// <summary>
        /// Base document for matching
        /// </summary>
        T Match { get; set; }

        /// <summary>
        /// Interesting terms extracted from base document
        /// </summary>
        IList<KeyValuePair<string, double>> InterestingTerms { get; set; }
    }
}
