using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet {
	public class SolrMultipleCriteriaQuery : ISolrQuery {
		private readonly string q;

		public class Operator {
			public const string OR = "OR";
			public const string AND = "AND";
		}

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery> queries): this(queries, "") {}

		public SolrMultipleCriteriaQuery(IEnumerable<ISolrQuery> queries, string oper) {
			q = Func.Join(string.Format(" {0} ", oper), queries, query => query.Query, true);
            if (!string.IsNullOrEmpty(q))
                q = "(" + q + ")";
		}

        public static SolrMultipleCriteriaQuery Create<T>(IEnumerable<T> queries) where T: ISolrQuery {            
            return new SolrMultipleCriteriaQuery(Func.Cast<ISolrQuery>(queries));
        }

		/// <summary>
		/// query string
		/// </summary>
		public string Query {
			get { return q; }
		}
	}
}