using System.Xml;

namespace SolrNet.Commands.Parameters {
	public interface ISolrDeleteParam {
		XmlNode ToXmlNode();
	}
}