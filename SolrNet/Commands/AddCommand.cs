using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Commands {
	/// <summary>
	/// Adds / updates documents to solr
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class AddCommand<T> : ISolrCommand where T : ISolrDocument {
		private readonly IEnumerable<T> documents = new List<T>();
		private ISolrDocumentSerializer<T> serializer = new SolrDocumentSerializer<T>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="documents">Documents to add</param>
		public AddCommand(IEnumerable<T> documents) {
			this.documents = documents;
		}

		/// <summary>
		/// Document serializer, default serializer should work fine for most cases
		/// </summary>
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