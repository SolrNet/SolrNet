namespace SolrNet {
    /// <summary>
    /// Response of the Extraction
    /// </summary>
    public class ExtractResponse {

        /// <summary>
        /// Operation response header
        /// </summary>
        public ResponseHeader ResponseHeader { get; set; }

        /// <summary>
        /// The content of the rich document used for ExtractingRequestHandler with extract only set to true
        /// http://wiki.apache.org/solr/TikaExtractOnlyExampleOutput 
        /// </summary>
        public string Content { get; set; }

        public ExtractResponse(ResponseHeader responseHeader) {
            ResponseHeader = responseHeader;
        }
    }
}