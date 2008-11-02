using System.Reflection;
using SolrNet.Attributes;

namespace SolrNet {
	public interface IUniqueKeyFinder<T> where T : ISolrDocument {
		PropertyInfo UniqueKeyProperty { get; }
		SolrUniqueKeyAttribute UniqueKeyAttribute { get; }
		string UniqueKeyFieldName { get; }
	}
}