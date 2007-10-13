using System;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class DSLTests {

		public class TestDocument: ISolrDocument {
			
		}

		[Test]
		public void tt() {
			Solr.Delete.ById("123456");
			Solr.Add(new TestDocument());
		}
	}

	public class Solr {
		public static DeleteParam Delete {
			get { return new DeleteParam(); }
		}

		public static void Add<T>(T t1) where T: ISolrDocument {
			throw new NotImplementedException();
		}
	}

	public class DeleteParam {
		public void ById(string id) {}

		public void ByQuery(ISolrQuery q) {}
	}
}