using System;
using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet {
	public class SolrMultipleCriteriaQuery: ISolrQuery {
		private readonly string q;

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery> queries) {
			q = Func.Join(" ", queries, query => query.Query, true);
		}

		/// <summary>
		/// query string
		/// </summary>
		public string Query {
			get { return q; }
		}
	}
}