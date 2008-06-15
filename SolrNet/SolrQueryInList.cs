using System.Collections;
using SolrNet.Utils;

namespace SolrNet {
	public class SolrQueryInList : ISolrQuery {
		private readonly string q;

		public SolrQueryInList(string fieldName, IEnumerable list) {
			q = "(" + Func.Join(" OR ", Func.Select(list, l => string.Format("{0}:{1}", fieldName, l))) + ")";
		}

		public string Query {
			get { return q; }
		}
	}
}