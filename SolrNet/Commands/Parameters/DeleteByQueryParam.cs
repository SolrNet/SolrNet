using System.Xml;

namespace SolrNet.Commands.Parameters {
	public class DeleteByQueryParam: ISolrDeleteParam {
		private readonly ISolrQuery query;

		public DeleteByQueryParam(ISolrQuery q) {
			query = q;
		}

		public XmlNode ToXmlNode() {
			var xml = new XmlDocument();
			XmlNode queryNode = xml.CreateElement("query");
			queryNode.InnerText = query.Query;
			return queryNode;
		}
	}
}