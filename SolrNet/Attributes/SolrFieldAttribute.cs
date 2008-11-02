using System;

namespace SolrNet.Attributes {
	/// <summary>
	/// Marks a property as present on solr. By default the field name is the property name
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SolrFieldAttribute : Attribute {
		public SolrFieldAttribute() {}

		/// <summary>
		/// Overrides field name
		/// </summary>
		/// <param name="fieldName"></param>
		public SolrFieldAttribute(string fieldName) {
			FieldName = fieldName;
		}

		/// <summary>
		/// Overrides field name
		/// </summary>
		public string FieldName { get; set; }
	}
}