using System;

namespace SolrNet.Exceptions {
    /// <summary>
    /// Invalid URL specified
    /// </summary>
	public class InvalidURLException : SolrNetException {
		public InvalidURLException(Exception innerException) : base(innerException) {}

		public InvalidURLException(string message) : base(message) {}

		public InvalidURLException(string message, Exception innerException) : base(message, innerException) {}

		public InvalidURLException() {}
	}
}