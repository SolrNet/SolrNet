namespace SolrNet
{
    /// <summary>
    /// Set of common headers used in tracing.
    /// </summary>
    public static class DiagnosticHeaders
    {
        /// <summary>
        /// The Activity Source name to subscribe to if you want to enable tracing by SolrNet.
        /// </summary>
        /// <code>
        /// Sdk.CreateTracerProviderBuilder()
        ///    .AddSource(SolrNet.DiagnosticHeaders.DefaultSourceName)
        ///    ...
        /// </code>
        public const string DefaultSourceName = "SolrNet";
        
        internal static class SemanticConventions
        {
            public const string DbSystem = "db.system";
            public const string DbQueryText = "db.query.text";
            public const string DbCollectionName = "db.collection.name";
            public const string DbOperationName = "db.operation.name";
            
            public const string HttpRequestMethod = "http.request.method";

            public const string ServerAddress = "server.address";
            public const string ServerPort = "server.port";

            public const string UrlFull = "url.full";
            
            public const string ErrorType = "error.type";
        }

        internal static class SolrNetConventions
        {
            public const string Status = "solr.status";
            public const string QTime = "solr.qtime";
        }
    }
}
