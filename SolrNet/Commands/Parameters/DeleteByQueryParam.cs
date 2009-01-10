using System.Xml;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Parameter to delete documents by query
    /// </summary>
	public class DeleteByQueryParam : ISolrDeleteParam {
		private readonly ISolrQuery query;

		public DeleteByQueryParam(ISolrQuery q) {
			query = q;
		}

		public XmlNode ToXmlNode() {
			var xml = new XmlDocument();
			var queryNode = xml.CreateElement("query");
			queryNode.InnerText = query.Query;
			return queryNode;
		}
	}
}