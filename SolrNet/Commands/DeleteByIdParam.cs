namespace SolrNet {
	public class DeleteByIdParam : ISolrDeleteParam {
		private string id;

		public DeleteByIdParam(string id) {
			this.id = id;
		}

		public string ToXmlString() {
			return string.Format("<id>{0}</id>", id);
		}
	}
}