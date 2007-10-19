using System;
using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet {
	public class SolrMultipleCriteriaQuery<T> : AbstractSolrQuery<T> where T : ISolrDocument {
		private string q;

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery<T>> queries) {
			q = Func.Join(" ", queries, new Converter<ISolrQuery<T>, string>(delegate(ISolrQuery<T> query) {
			                                                                 	return query.Query;
			                                                                 }), true);
		}

		/// <summary>
		/// query string
		/// </summary>
		public override string Query {
			get { return q; }
		}
	}
}