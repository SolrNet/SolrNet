using SolrNet.Tests;

namespace SolrNet {
	public class DeleteByIdParam : ISolrDeleteParam {
		private string id;

		public DeleteByIdParam(string id) {
			this.id = id;
		}

		public override string ToString() {
			return string.Format("<id>{0}</id>", id);
		}
	}
}