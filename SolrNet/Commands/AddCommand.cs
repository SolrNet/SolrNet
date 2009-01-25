using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Commands {
	/// <summary>
	/// Adds / updates documents to solr
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class AddCommand<T> : ISolrCommand {
		private readonly IEnumerable<T> documents = new List<T>();
	    private readonly ISolrDocumentSerializer<T> documentSerializer;

	    /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="documents">Documents to add</param>
		/// <param name="serializer">document serializer</param>
		public AddCommand(IEnumerable<T> documents, ISolrDocumentSerializer<T> serializer) {
	        this.documents = documents;
	        documentSerializer = serializer;
		}

	    public string Execute(ISolrConnection connection) {
			var xml = new XmlDocument();
			var addElement = xml.CreateElement("add");
			foreach (T doc in documents) {
                var xmlDoc = documentSerializer.Serialize(doc);
				addElement.AppendChild(xml.ImportNode(xmlDoc.DocumentElement, true));
			}
			xml.AppendChild(addElement);
			return connection.Post("/update", xml.OuterXml);
		}
	}
}