using System;
using System.Xml.Linq;

namespace SolrNet.Impl {
    /// <summary>
    /// Parses a chunk of a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public interface ISolrAbstractResponseParser<T> {
        /// <summary>
        /// Parses a chunk of a query response into the results object
        /// </summary>
        /// <param name="xml">query response</param>
        /// <param name="results">results object</param>
        void Parse(XDocument xml, AbstractSolrQueryResults<T> results);
    }
}
