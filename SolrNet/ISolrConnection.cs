using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SolrNet {
	public interface ISolrConnection {
		string ServerURL { get; set; }
		Encoding XmlEncoding { get; set;}
		string Post(string s);
		XmlDocument PostXml(XmlDocument xml);
		string Get(string relativeUrl, IDictionary<string, string> parameters);
	}
}