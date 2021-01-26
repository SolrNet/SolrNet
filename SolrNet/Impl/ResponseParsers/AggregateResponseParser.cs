using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SolrNet.Impl.ResponseParsers {
    public class AggregateResponseParser<T> : ISolrAbstractResponseParser<T> {
        private readonly IEnumerable<ISolrAbstractResponseParser<T>> parsers;

        public AggregateResponseParser(IEnumerable<ISolrAbstractResponseParser<T>> parsers) {
            this.parsers = parsers;
        }

        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            foreach (var p in parsers)
                p.Parse(xml, results);
        }
    }
}
