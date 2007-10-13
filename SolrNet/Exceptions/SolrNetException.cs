using System;

namespace SolrNet.Exceptions {
	public class SolrNetException : ApplicationException {
		public SolrNetException(Exception innerException) : base(innerException.Message, innerException) {}
		public SolrNetException(string message) : base(message) {}

		public SolrNetException() {}

		public SolrNetException(string message, Exception innerException) : base(message, innerException) {}
	}
}