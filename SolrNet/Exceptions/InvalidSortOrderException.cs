using System;

namespace SolrNet.Exceptions {
    /// <summary>
    /// Error parsing <see cref="SortOrder"/>
    /// </summary>
    public class InvalidSortOrderException : SolrNetException {
        public InvalidSortOrderException() {}
        public InvalidSortOrderException(string message) : base(message) {}
        public InvalidSortOrderException(string message, Exception innerException) : base(message, innerException) {}
        public InvalidSortOrderException(Exception innerException) : base(innerException) {}
    }
}