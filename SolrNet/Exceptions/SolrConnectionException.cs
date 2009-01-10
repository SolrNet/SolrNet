using System;

namespace SolrNet.Exceptions {
    /// <summary>
    /// Error connecting to Solr. See inner exception for more information.
    /// </summary>
	public class SolrConnectionException : SolrNetException {
		public SolrConnectionException(string message) : base(message) {}
		public SolrConnectionException(Exception innerException) : base(innerException) {}
		public SolrConnectionException(string message, Exception innerException) : base(message, innerException) {}
		public SolrConnectionException() {}
	}
}