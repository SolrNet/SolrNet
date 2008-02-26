using System;
using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet {
	public class SolrMultipleCriteriaQuery<T> : ISolrQuery where T : ISolrDocument {
		private readonly string q;

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery> queries) {
			q = Func.Join(" ", queries, delegate(ISolrQuery query) {
			                                                          	return query.Query;
			}, true);
		}

		/// <summary>
		/// query string
		/// </summary>
		public string Query {
			get { return q; }
		}
	}
}