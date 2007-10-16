using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SolrNet {
	public interface ISolrConnection {
		string ServerURL { get; set; }
		string Version { get; set; }
		Encoding XmlEncoding { get; set; }
		string Post(string relativeUrl, string s);
		XmlDocument PostXml(string relativeUrl, XmlDocument xml);
		string Get(string relativeUrl, IDictionary<string, string> parameters);
	}
}