using System;

namespace SolrNet {
	/// <summary>
	/// Marks a property as present on solr. By default the field name is the property name
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SolrField : Attribute {
		private string fieldName;

		public SolrField() {}

		/// <summary>
		/// Overrides field name
		/// </summary>
		/// <param name="fieldName"></param>
		public SolrField(string fieldName) {
			FieldName = fieldName;
		}

		/// <summary>
		/// Overrides field name
		/// </summary>
		public string FieldName {
			get { return fieldName; }
			set { fieldName = value; }
		}
	}
}