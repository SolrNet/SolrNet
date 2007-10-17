using System.Reflection;
using SolrNet.Exceptions;

namespace SolrNet {
	public class UniqueKeyFinder<T> : IUniqueKeyFinder<T> where T : ISolrDocument {
		private bool? hasUniqueKeyProperty = null;
		private PropertyInfo uniqueKeyProperty;

		public PropertyInfo UniqueKeyProperty {
			get {
				if (!hasUniqueKeyProperty.HasValue) {
					hasUniqueKeyProperty = false;
					foreach (PropertyInfo property in typeof (T).GetProperties()) {
						object[] atts = property.GetCustomAttributes(typeof (SolrUniqueKey), true);
						if (atts.Length > 0) {
							if (uniqueKeyProperty != null)
								throw new BadMappingException("Only one SolrUniqueKey allowed per document class");
							uniqueKeyProperty = property;
							hasUniqueKeyProperty = true;
						}
					}
				}
				if (hasUniqueKeyProperty.Value)
					return uniqueKeyProperty;
				return null;
			}
		}
	}
}