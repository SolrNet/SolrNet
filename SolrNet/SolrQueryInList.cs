using SolrNet.Utils;
using System.Collections.Generic;

namespace SolrNet {
	public class SolrQueryInList : ISolrQuery {
		private readonly string q;

		public SolrQueryInList(string fieldName, IEnumerable<string> list) {
			q = "(" + Func.Join(" OR ", Func.Select(list, l => new SolrQueryByField(fieldName, l).Query)) + ")";
		}

		public string Query {
			get { return q; }
		}
	}
}