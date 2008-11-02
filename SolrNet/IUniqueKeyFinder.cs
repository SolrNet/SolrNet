using System.Reflection;
using SolrNet.Attributes;

namespace SolrNet {
	public interface IUniqueKeyFinder<T>  {
		PropertyInfo UniqueKeyProperty { get; }
		SolrUniqueKeyAttribute UniqueKeyAttribute { get; }
		string UniqueKeyFieldName { get; }
	}
}