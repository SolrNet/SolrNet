using System;

namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// Request handler parameters
    /// </summary>
    public class RequestHandlerParameters
    {
        private readonly string handlerUrl;

        /// <summary>
        /// URL for query request handler.
        /// </summary>
        public string HandlerUrl
        {
            get { return handlerUrl; }
        }

        public RequestHandlerParameters(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");
            this.handlerUrl = url;
        }
    }
}