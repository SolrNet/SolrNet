using System.Collections.Generic;

namespace SolrNet {
	public class ResponseHeader {
		public int Status { get; set; }
		public int QTime { get; set; }
		public IDictionary<string, string> Params { get; set; }
	}
}