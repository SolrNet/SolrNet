using System.Xml;

namespace SolrNet {
	public interface ISolrDeleteParam {
		XmlNode ToXmlNode();
	}
}