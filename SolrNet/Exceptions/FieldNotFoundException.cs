using System;

namespace SolrNet.Exceptions {
    /// <summary>
    /// OBSOLETE
    /// </summary>
    [Obsolete("Fields not mapped are now simply ignored")]
	public class FieldNotFoundException : BadMappingException {
		public string FieldName { get; set; }

		public FieldNotFoundException() {}
		public FieldNotFoundException(string message, Exception innerException) : base(message, innerException) {}
		public FieldNotFoundException(string message) : base(message) {}
		public FieldNotFoundException(Exception innerException) : base(innerException) {}
	}
}