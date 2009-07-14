using System.Collections.Generic;

namespace SolrNet.Impl {
    public class SolrDocumentIndexer<T> : ISolrDocumentIndexer<T> {
        private readonly IReadOnlyMappingManager mappingManager;

        public SolrDocumentIndexer(IReadOnlyMappingManager mappingManager) {
            this.mappingManager = mappingManager;
        }

        /// <summary>
        /// Creates an index of documents by unique key
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public IDictionary<string, T> IndexResultsByKey(IEnumerable<T> results) {
            var r = new Dictionary<string, T>();
            var prop = mappingManager.GetUniqueKey(typeof (T)).Key;
            foreach (var d in results) {
                var key = prop.GetValue(d, null).ToString();
                r[key] = d;
            }
            return r;
        }
    }
}