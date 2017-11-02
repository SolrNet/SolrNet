using System;
using System.Xml.Linq;
using SolrNet.Impl;

namespace SolrNet.Tests.Mocks {
    public class MSolrExtractResponseParser : ISolrExtractResponseParser {
        public Func<XDocument, ExtractResponse> parse;

        public ExtractResponse Parse(XDocument response) {
            return parse(response);
        }
    }
}