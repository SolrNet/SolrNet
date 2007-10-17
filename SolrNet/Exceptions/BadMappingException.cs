using System;
using SolrNet.Exceptions;

namespace SolrNet.Exceptions {
	public class BadMappingException: SolrNetException {
		public BadMappingException(Exception innerException) : base(innerException) {}
		public BadMappingException(string message) : base(message) {}
		public BadMappingException() {}
		public BadMappingException(string message, Exception innerException) : base(message, innerException) {}
	}
}