using System.Collections.Generic;

namespace SolrNet.Impl {
    public interface ISolrDocumentIndexer<T> {
        IDictionary<string, T> IndexResultsByKey(IEnumerable<T> results);
    }
}