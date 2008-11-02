using SolrNet.Attributes;

namespace SolrNet.Tests {
	public class SolrFieldExample {
		public class Test {
			[SolrField]
			public string something {
				get { return ""; }
			}

			[SolrField("someName")]
			public string somethingWithName {
				get { return ""; }
			}
		}
	}
}