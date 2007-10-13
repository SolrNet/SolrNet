using System.Collections.Generic;
using System.Text;

namespace SolrNet {
	public class AddCommand<T> : ISolrCommand where T : ISolrDocument {
		private IEnumerable<T> documents = new List<T>();
		private ISolrDocumentSerializer<T> serializer = new SolrDocumentSerializer<T>();

		public AddCommand(IEnumerable<T> documents) {
			this.documents = documents;
		}

		public ISolrDocumentSerializer<T> Serializer {
			get { return serializer; }
			set { serializer = value; }
		}

		public void Execute(ISolrConnection connection) {
			StringBuilder sb = new StringBuilder();
			sb.Append("<add>"); // HACK use xml
			foreach (T doc in documents) {
				sb.Append(serializer.Serialize(doc));
			}
			sb.Append("</add>");
			connection.Post(sb.ToString());
		}
	}
}