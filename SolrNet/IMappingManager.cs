using System.Reflection;

namespace SolrNet {
	public interface IMappingManager : IReadOnlyMappingManager {
		void Add(PropertyInfo property);
		void Add(PropertyInfo property, string fieldName);

		void SetUniqueKey(PropertyInfo property);
	}
}