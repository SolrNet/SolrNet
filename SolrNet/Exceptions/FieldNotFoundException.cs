using System;

namespace SolrNet.Exceptions {
	public class FieldNotFoundException : BadMappingException {
		public string FieldName { get; set; }

		public FieldNotFoundException() {}
		public FieldNotFoundException(string message, Exception innerException) : base(message, innerException) {}
		public FieldNotFoundException(string message) : base(message) {}
		public FieldNotFoundException(Exception innerException) : base(innerException) {}
	}
}