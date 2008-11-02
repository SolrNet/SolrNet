using System.Reflection;
using SolrNet.Attributes;
using SolrNet.Exceptions;

namespace SolrNet {
	public class UniqueKeyFinder<T> : IUniqueKeyFinder<T> {
		private int? uniqueKeyCount;
		private PropertyInfo prop;
		private SolrUniqueKeyAttribute attr;

		public SolrUniqueKeyAttribute UniqueKeyAttribute {
			get {
				if (!uniqueKeyCount.HasValue) {
					uniqueKeyCount = 0;
					foreach (var property in typeof (T).GetProperties()) {
						var atts = property.GetCustomAttributes(typeof (SolrUniqueKeyAttribute), true);
						if (atts.Length > 0) {
							uniqueKeyCount++;
							if (prop != null) {
								break;
							}
							attr = (SolrUniqueKeyAttribute) atts[0];
							prop = property;
						}
					}
				}
				if (uniqueKeyCount > 1)
					throw new BadMappingException("Only one SolrUniqueKey allowed per document class");
				if (uniqueKeyCount == 1)
					return attr;
				return null;
			}
		}

		public string UniqueKeyFieldName {
			get {
				var a = UniqueKeyAttribute;
				if (a == null)
					return null;
				if (a.FieldName != null)
					return a.FieldName;
				return prop.Name;
			}
		}

		public PropertyInfo UniqueKeyProperty {
			get {
				var a = UniqueKeyAttribute;
				if (a == null)
					return null;
				return prop;
			}
		}
	}
}