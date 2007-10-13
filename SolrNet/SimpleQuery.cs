using System;

namespace SolrNet.Tests {
	public class SimpleQuery<T> : ISolrQuery<T> {
		public string Query {
			get { throw new NotImplementedException(); }
		}
	}
}