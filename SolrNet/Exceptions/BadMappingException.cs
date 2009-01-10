using System;

namespace SolrNet.Exceptions {
    /// <summary>
    /// Base exception for mapping errors
    /// </summary>
	public class BadMappingException : SolrNetException {
		public BadMappingException(Exception innerException) : base(innerException) {}
		public BadMappingException(string message) : base(message) {}
		public BadMappingException() {}
		public BadMappingException(string message, Exception innerException) : base(message, innerException) {}
	}
}