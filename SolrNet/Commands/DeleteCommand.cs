namespace SolrNet.Tests {
	public class DeleteCommand : ISolrCommand {
		private bool fromPending = true;
		private bool fromCommitted = true;
		private ISolrDeleteParam deleteParam;

		public bool FromPending {
			get { return fromPending; }
			set { fromPending = value; }
		}

		public bool FromCommitted {
			get { return fromCommitted; }
			set { fromCommitted = value; }
		}

		public ISolrDeleteParam DeleteParam {
			get { return deleteParam; }
			set { deleteParam = value; }
		}

		public string Execute(ISolrConnection connection) {
			return connection.Post(string.Format("<delete>{0}</delete>", deleteParam.ToXmlString()));
		}
	}
}