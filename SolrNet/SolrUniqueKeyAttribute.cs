using System;

namespace SolrNet {
	[AttributeUsage(AttributeTargets.Property)]
	public class SolrUniqueKeyAttribute : Attribute {
		/// <summary>
		/// Overrides field name
		/// </summary>
		public string FieldName { get; set; }

		public SolrUniqueKeyAttribute() {}

		public SolrUniqueKeyAttribute(string fieldName) {
			FieldName = fieldName;
		}
	}
}