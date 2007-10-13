using System.Collections.Generic;
using System.Xml;

namespace SolrNet {
	public interface ISolrConnection {
		string ServerURL { get; set; }
		string Post(string s, string contentType);
		XmlDocument PostXml(XmlDocument xml);
		string Get(string relativeUrl, IDictionary<string, string> parameters);
	}
}