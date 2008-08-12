using System.Collections.Generic;

namespace SolrNet {
	public class SolrFacetFieldQuery : ISolrFacetQuery {
		private readonly string field;

		public SolrFacetFieldQuery(string field) {
			this.field = field;
		}

		public string Prefix { get; set; }
		public bool? Sort { get; set; }
		public int? Limit { get; set; }
		public int? Offset { get; set; }
		public int? MinCount { get; set; }
		public bool? Missing { get; set; }
		public int? EnumCacheMinDf { get; set; }

		public IEnumerable<KeyValuePair<string, string>> Query {
			get {
				var r = new List<KeyValuePair<string, string>> {
					new KeyValuePair<string, string>("facet.field", field)
				};
				if (Prefix != null)
					r.Add(new KeyValuePair<string, string>("facet.prefix", Prefix));
				if (Sort.HasValue)
					r.Add(new KeyValuePair<string, string>("facet.sort", Sort.ToString().ToLowerInvariant()));
				if (Limit.HasValue)
					r.Add(new KeyValuePair<string, string>("facet.limit", Limit.ToString()));
				if (Offset.HasValue)
					r.Add(new KeyValuePair<string, string>("facet.offset", Offset.ToString()));
				if (MinCount.HasValue)
					r.Add(new KeyValuePair<string, string>("facet.mincount", MinCount.ToString()));
				if (EnumCacheMinDf.HasValue)
					r.Add(new KeyValuePair<string, string>("facet.enum.cache.minDf", EnumCacheMinDf.ToString()));
				return r;
			}
		}
	}
}