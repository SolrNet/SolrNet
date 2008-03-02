using System;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	public class InvalidSortOrderException : SolrNetException {
		public InvalidSortOrderException() {}
		public InvalidSortOrderException(string message) : base(message) {}
		public InvalidSortOrderException(string message, Exception innerException) : base(message, innerException) {}
		public InvalidSortOrderException(Exception innerException) : base(innerException) {}
	}
}