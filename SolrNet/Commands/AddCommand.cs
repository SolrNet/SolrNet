using System.Collections.Generic;
using System.Xml;

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

		public string Execute(ISolrConnection connection) {
			XmlDocument xml = new XmlDocument();
			XmlElement addElement = xml.CreateElement("add");
			foreach (T doc in documents) {
				XmlDocument xmlDoc = serializer.Serialize(doc);
				addElement.AppendChild(xml.ImportNode(xmlDoc.DocumentElement, true));
			}
			xml.AppendChild(addElement);
			return connection.Post("/update", xml.OuterXml);
		}
	}
}