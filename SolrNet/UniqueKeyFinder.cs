using System.Reflection;
using SolrNet.Exceptions;

namespace SolrNet {
	public class UniqueKeyFinder<T> : IUniqueKeyFinder<T> where T : ISolrDocument {
		private int? uniqueKeyCount = null;
		private PropertyInfo uniqueKeyProperty;

		public PropertyInfo UniqueKeyProperty {
			get {
				if (!uniqueKeyCount.HasValue) {
					uniqueKeyCount = 0;
					foreach (PropertyInfo property in typeof (T).GetProperties()) {
						object[] atts = property.GetCustomAttributes(typeof (SolrUniqueKeyAttribute), true);
						if (atts.Length > 0) {
							uniqueKeyCount++;
							if (uniqueKeyProperty != null) {
								break;
							}
							uniqueKeyProperty = property;
						}
					}
				}
				if (uniqueKeyCount > 1)
					throw new BadMappingException("Only one SolrUniqueKey allowed per document class");
				else if (uniqueKeyCount == 1)
					return uniqueKeyProperty;
				return null;
			}
		}
	}
}