using System.Xml;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Delete command parameter
    /// </summary>
	public interface ISolrDeleteParam {
		XmlNode ToXmlNode();
	}
}