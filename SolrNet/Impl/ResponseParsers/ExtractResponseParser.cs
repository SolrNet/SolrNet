using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers
{
    public class ExtractResponseParser: ISolrExtractResponseParser {
        private readonly ISolrHeaderResponseParser headerResponseParser;

        public ExtractResponseParser(ISolrHeaderResponseParser headerResponseParser) {
            this.headerResponseParser = headerResponseParser;
        }

        /// <inheritdoc />
        public ExtractResponse Parse(XDocument response) {
            var responseHeader = headerResponseParser.Parse(response);

            var contentNode = response.Element("response").Element("str");
            var extractResponse = new ExtractResponse(responseHeader) {
                Content = contentNode != null ? contentNode.Value : null
            };

            extractResponse.Metadata = ParseMetadata(response);

            return extractResponse;
        }
    
        /// Metadata looks like this:
        /// <response>
        ///     <lst name="null_metadata">
        ///         <arr name="stream_source_info">
        ///             <null />
        ///         </arr>
        ///         <arr name="nbTab">
        ///             <str>10</str>
        ///         </arr>
        ///         <arr name="date">
        ///             <str>2009-06-24T15:25:00</str>
        ///         </arr>
        ///     </lst>
        /// </response>
        private List<ExtractField> ParseMetadata(XDocument response)
        {

            var metadataElements = response.Element("response")
                .Elements("lst")
                .Where(X.AttrEq("name", "null_metadata"))
                .SelectMany(x => x.Elements("arr"));

            var metadata = new List<ExtractField>(metadataElements.Count());
            foreach (var node in metadataElements)
            {
                var nameAttrib = node.Attribute("name");
                if (nameAttrib == null)
                {
                    throw new NotSupportedException("Metadata node has no name attribute: " + node);
                }

                // contents of the <arr> element might be a <str/> or a <null/>
                string fieldValue;
                var stringValue = node.Element("str");
                if (stringValue != null)
                {
                    // is a <str/> node
                    fieldValue = stringValue.Value;
                }
                else if (node.Element("null") != null)
                {
                    // is a <null/> node
                    fieldValue = null;
                }
                else
                {
                    throw new NotSupportedException("No support for metadata element type: " + node);
                }

                metadata.Add(new ExtractField(nameAttrib.Value, fieldValue));
            }

            return metadata;
        }
    }
}
