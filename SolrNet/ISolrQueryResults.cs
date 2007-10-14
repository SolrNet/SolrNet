using System.Collections.Generic;

namespace SolrNet {
	public interface ISolrQueryResults<T> : ICollection<T> where T : ISolrDocument { }
}