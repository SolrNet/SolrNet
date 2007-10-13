using System;

namespace SolrNet {
	[AttributeUsage(AttributeTargets.Property)]
	public class SolrField : Attribute {
		private string fieldName;

		public SolrField() {}

		public SolrField(string fieldName) {
			FieldName = fieldName;
		}

		public string FieldName {
			get { return fieldName; }
			set { fieldName = value; }
		}
	}
}