using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl.FacetQuerySerializers
{
	public class SolrFacetPivotQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetPivotQuery> 
	{
		private static KeyValuePair<K, V> KV<K, V>(K key, V value)
		{
			return new KeyValuePair<K, V>(key, value);
		}

		public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetPivotQuery q)
		{
			foreach (var pivotQ in q.Fields)
			{
				if (string.IsNullOrEmpty(pivotQ))
					continue;
				yield return KV("facet.pivot", pivotQ);
			}
			if (q.MinCount.HasValue)
			{
				yield return KV("facet.pivot.mincount", q.MinCount.ToString());
			}
		}

	}
}
