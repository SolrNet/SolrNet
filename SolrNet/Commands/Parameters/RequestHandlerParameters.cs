using System;

namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// Request handler parameters
    /// </summary>
    public class RequestHandlerParameters
    {

        /// <summary>
        /// URL for query request handler.
        /// </summary>
        public string HandlerUrl { get; }

        public RequestHandlerParameters(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");
            this.HandlerUrl = url;
        }
    }
}