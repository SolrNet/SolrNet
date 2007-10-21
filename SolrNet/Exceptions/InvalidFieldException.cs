using System;

namespace SolrNet.Exceptions {
	public class InvalidFieldException : SolrNetException {
		public InvalidFieldException(Exception innerException) : base(innerException) {}
		public InvalidFieldException(string message) : base(message) {}
		public InvalidFieldException(string message, Exception innerException) : base(message, innerException) {}
		public InvalidFieldException() {}
	}
}