using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
	public class QueryOptions {
		public int? Start { get; set; }
		public int? Rows { get; set; }
		public ICollection<SortOrder> OrderBy { get; set; }

		public QueryOptions() {
			OrderBy = new List<SortOrder>();
		}
	}
}