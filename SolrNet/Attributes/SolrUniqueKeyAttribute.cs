using System;

namespace SolrNet.Attributes {
	[AttributeUsage(AttributeTargets.Property)]
	public class SolrUniqueKeyAttribute : SolrFieldAttribute {
		public SolrUniqueKeyAttribute() {}
		public SolrUniqueKeyAttribute(string fieldName) : base(fieldName) {}
	}
}