using System.Xml;

namespace SolrNet {
	public interface ISolrDocumentSerializer<T> where T : ISolrDocument {
		XmlDocument Serialize(T doc);
	}
}