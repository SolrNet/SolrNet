using System.Xml;

namespace SolrNet.Commands.Parameters {
	public class DeleteByIdParam : ISolrDeleteParam {
		private readonly string id;

		public DeleteByIdParam(string id) {
			this.id = id;
		}

		public XmlNode ToXmlNode() {
			var xml = new XmlDocument();
			var idNode = xml.CreateElement("id");
			idNode.InnerText = id;
			return idNode;
		}
	}
}