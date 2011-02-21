using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers
{
    public class ExtractResponseParser: ISolrExtractResponseParser {
        private readonly ISolrHeaderResponseParser headerResponseParser;

        public ExtractResponseParser(ISolrHeaderResponseParser headerResponseParser) {
            this.headerResponseParser = headerResponseParser;
        }

        public ExtractResponse Parse(XDocument response) {
            var responseHeader = headerResponseParser.Parse(response);

            var contentNode = response.XPathSelectElement("response/str");
            var extractResponse = new ExtractResponse(responseHeader) {
                Content = contentNode != null ? contentNode.Value : null
            };

            return extractResponse;
        }
    }
}
