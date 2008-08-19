using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SolrNet;

namespace SolrNet.DSL.Tests {
	public class MockConnection : ISolrConnection {
		private readonly IDictionary<string, string> expectations;
		private const string response =
	@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc></doc></result>
</response>
";

		public MockConnection() {}

		public MockConnection(IDictionary<string, string> expectations) {
			this.expectations = expectations;
		}

		public virtual string ServerURL { get; set; }

		public virtual string Version { get; set; }

		public virtual Encoding XmlEncoding { get; set; }

		public virtual string Post(string relativeUrl, string s) {
			return string.Empty;
		}

		public virtual string Get(string relativeUrl, IDictionary<string, string> parameters) {
			Assert.AreEqual(expectations.Count, parameters.Count, "Expected {0} parameters but found {1}", expectations.Count, parameters.Count);
			foreach (var p in parameters)
				Assert.IsTrue(expectations.Contains(p), "Parameter {0}:{1}, not found in expectations", p.Key, p.Value);
			return response;
		}
	}
}