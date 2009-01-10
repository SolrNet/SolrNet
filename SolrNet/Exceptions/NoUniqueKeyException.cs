using System;

namespace SolrNet.Exceptions {
    /// <summary>
    /// No unique key found (either mapping or value) when one was required.
    /// </summary>
	public class NoUniqueKeyException : SolrNetException {
		public NoUniqueKeyException(string message) : base(message) {}
		public NoUniqueKeyException(string message, Exception innerException) : base(message, innerException) {}
		public NoUniqueKeyException(Exception innerException) : base(innerException) {}
		public NoUniqueKeyException() {}
	}
}