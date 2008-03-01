using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class FacetsTests {
		public class SolrDoc: ISolrDocument {
			
		}

		[Test]
		public void FacetQueriesResults() {
			var results = new SolrQueryResults<SolrDoc>();
			results.FacetQueries["hotdeal:true"] = 1;
		}

		[Test]
		public void FacetFieldsResults() {
			var results = new SolrQueryResults<SolrDoc>();
			results.FacetFields["hotdeal"] = 1;			
		}
	}
}
