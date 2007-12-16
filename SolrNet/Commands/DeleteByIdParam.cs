using System.Xml;

namespace SolrNet {
	public class DeleteByIdParam : ISolrDeleteParam {
		private readonly string id;

		public DeleteByIdParam(string id) {
			this.id = id;
		}

		public XmlNode ToXmlNode() {
			XmlDocument xml = new XmlDocument();
			XmlNode idNode = xml.CreateElement("id");
			idNode.InnerText = id;
			return idNode;
		}
	}
}