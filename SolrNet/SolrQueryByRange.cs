namespace SolrNet {
	public class SolrQueryByRange<T, RT> : ISolrQuery<T> where T : ISolrDocument {
		private readonly string q;
		public SolrQueryByRange(string fieldName, RT from, RT to) : this(fieldName, from, to, true) {}

		public SolrQueryByRange(string fieldName, RT from, RT to, bool inclusive) {
			q = "$field:$ii$from TO $to$if"
				.Replace("$field", fieldName)
				.Replace("$ii", inclusive ? "[" : "{")
				.Replace("$if", inclusive ? "]" : "}")
				.Replace("$from", from.ToString())
				.Replace("$to", to.ToString());
		}

		/// <summary>
		/// query string
		/// </summary>
		public string Query {
			get { return q; }
		}
	}
}