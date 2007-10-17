using System;
using SolrNet.Exceptions;

namespace SolrNet.Exceptions {
	public class NoUniqueKeyException: SolrNetException {
		public NoUniqueKeyException(string message) : base(message) {}
		public NoUniqueKeyException(string message, Exception innerException) : base(message, innerException) {}
		public NoUniqueKeyException(Exception innerException) : base(innerException) {}
		public NoUniqueKeyException() {}
	}
}