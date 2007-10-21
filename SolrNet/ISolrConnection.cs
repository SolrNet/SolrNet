using System.Collections.Generic;
using System.Text;

namespace SolrNet {
	public interface ISolrConnection {
		string ServerURL { get; set; }
		string Version { get; set; }
		Encoding XmlEncoding { get; set; }
		string Post(string relativeUrl, string s);
		string Get(string relativeUrl, IDictionary<string, string> parameters);
	}
}