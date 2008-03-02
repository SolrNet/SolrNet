using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
	public class QueryOptions : ExecuteOptions {
		private ICollection<SortOrder> orderBy = new List<SortOrder>();

		public ICollection<SortOrder> OrderBy {
			get { return orderBy; }
			set { orderBy = value; }
		}
	}
}