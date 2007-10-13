using System;
using System.Collections.Generic;
using System.Text;

namespace SolrNet.Tests {
	public class SolrFieldExample {
		public class Test {
			[SolrField]
			public string something {
				get {
					return "";
				}
			}

			[SolrField("someName")]
			public string somethingWithName {
				get {
					return "";
				}
			}
		}
	}
}
