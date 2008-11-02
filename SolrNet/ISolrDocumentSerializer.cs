using System.Xml;

namespace SolrNet {
	/// <summary>
	/// Serializes a solr document to xml. 
	/// </summary>
	/// <typeparam name="T">document type</typeparam>
	public interface ISolrDocumentSerializer<T> {
		/// <summary>
		/// Serializes a Solr document to xml
		/// </summary>
		/// <param name="doc">document to serialize</param>
		/// <returns>serialized document</returns>
		XmlDocument Serialize(T doc);

		/* TODO maybe I should use XmlSerializer (http://msdn2.microsoft.com/en-us/library/system.xml.serialization.xmlserializer(vs.80).aspx)
		 * or attributes http://msdn2.microsoft.com/en-us/library/83y7df3e(VS.80).aspx
		 * or IXmlSerializable http://msdn2.microsoft.com/en-us/library/system.xml.serialization.ixmlserializable(VS.80).aspx
		 * but I don't want to put that burden on the document classes, they should be as POCO as possible
		 */
	}
}