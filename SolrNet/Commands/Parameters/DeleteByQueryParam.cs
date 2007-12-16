using System.Xml;

namespace SolrNet.Commands.Parameters {
	public class DeleteByQueryParam<T> : ISolrDeleteParam where T : ISolrDocument {
		private readonly ISolrQuery<T> query;

		public DeleteByQueryParam(ISolrQuery<T> q) {
			query = q;
		}

		public XmlNode ToXmlNode() {
			XmlDocument xml = new XmlDocument();
			XmlNode queryNode = xml.CreateElement("query");
			queryNode.InnerText = query.Query;
			return queryNode;
		}
	}
}