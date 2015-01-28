using System;
using System.Xml.Linq;
using Moroco;
using SolrNet.Impl;

namespace SolrNet.Tests.Mocks {
    public class MSolrHeaderResponseParser : ISolrHeaderResponseParser {
        public MFunc<XDocument, ResponseHeader> parse;

        public ResponseHeader Parse(XDocument response) {
            return parse.Invoke(response);
        }
    }
}