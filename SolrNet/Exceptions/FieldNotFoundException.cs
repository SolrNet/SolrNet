using System;

namespace SolrNet.Exceptions {
	public class FieldNotFoundException : BadMappingException {
		private string fieldName;

		public string FieldName {
			get { return fieldName; }
			set { fieldName = value; }
		}

		public FieldNotFoundException() {}
		public FieldNotFoundException(string message, Exception innerException) : base(message, innerException) {}
		public FieldNotFoundException(string message) : base(message) {}
		public FieldNotFoundException(Exception innerException) : base(innerException) {}
	}
}