using System;

namespace SolrNet {
	public class SolrQueryResultParser<T> : ISolrQueryResultParser<T> where T : ISolrDocument {
		public ISolrQueryResults<T> Parse(string r) {
			throw new NotImplementedException();
		}
	}
}