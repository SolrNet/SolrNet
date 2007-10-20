namespace SolrNet {
	public class DeleteCommand : ISolrCommand {
		private bool? fromPending;
		private bool? fromCommitted;
		private ISolrDeleteParam deleteParam;

		public DeleteCommand(ISolrDeleteParam deleteParam) {
			this.deleteParam = deleteParam;
		}

		public bool? FromPending {
			get { return fromPending; }
			set { fromPending = value; }
		}

		public bool? FromCommitted {
			get { return fromCommitted; }
			set { fromCommitted = value; }
		}

		public ISolrDeleteParam DeleteParam {
			get { return deleteParam; }
		}

		public string Execute(ISolrConnection connection) {
			// TODO use proper xml
			return connection.Post("/update", 
				string.Format("<delete{1}{2}>{0}</delete>", 
					deleteParam.ToXmlString(),
					fromPending.HasValue ? string.Format(" fromPending=\"{0}\"", fromPending.Value.ToString().ToLower()) : "",
					fromCommitted.HasValue ? string.Format(" fromCommitted=\"{0}\"", fromCommitted.Value.ToString().ToLower()) : ""));
		}
	}
}