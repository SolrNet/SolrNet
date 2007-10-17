using System;
using SolrNet.Exceptions;

namespace SolrNet.Exceptions {
	public class SolrConnectionException: SolrNetException {
		public SolrConnectionException(string message) : base(message) {}
		public SolrConnectionException(Exception innerException) : base(innerException) {}
		public SolrConnectionException(string message, Exception innerException) : base(message, innerException) {}
		public SolrConnectionException() {}
	}
}