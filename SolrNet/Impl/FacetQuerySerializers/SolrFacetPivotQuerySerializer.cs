using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet.Impl.FacetQuerySerializers {
    /// <summary>
    /// Serializes <see cref="SolrFacetPivotQuery"/>
    /// </summary>
    public class SolrFacetPivotQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetPivotQuery> {
        /// <summary>
        /// Serializes <see cref="SolrFacetPivotQuery"/>
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetPivotQuery q) {
            foreach (var pivotQ in q.Fields) {
                if (string.IsNullOrEmpty(pivotQ))
                    continue;
                yield return KV.Create("facet.pivot", pivotQ);
            }
            if (q.MinCount.HasValue)
                yield return KV.Create("facet.pivot.mincount", q.MinCount.ToString());
        }
    }
}