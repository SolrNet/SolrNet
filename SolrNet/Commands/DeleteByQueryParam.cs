using System.Xml;

namespace SolrNet.Tests {
	public class DeleteByQueryParam<T> : ISolrDeleteParam where T : ISolrDocument {
		private ISolrQuery<T> query;

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