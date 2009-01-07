using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Commands;

namespace SolrNet.Tests {
	[TestFixture]
	public class AddCommandTests {
		public class SampleDoc : ISolrDocument {
			[SolrField]
			public string Id {
				get { return "id"; }
			}

			[SolrField("Flower")]
			public decimal caca {
				get { return 23.5m; }
			}
		}

		public class TestDocWithCollections : ISolrDocument {
			[SolrField]
			public ICollection<string> coll {
				get { return new[] {"one", "two"}; }
			}
		}

		public delegate string Writer(string ignored, string s);

		[Test]
		public void Execute() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				conn.Post("/update",
				          "<add><doc><field name=\"Id\">id</field><field name=\"Flower\">23.5</field></doc></add>");
				LastCall.Repeat.Once().Do(new Writer(delegate(string ignored, string s) {
					Console.WriteLine(s);
					return null;
				}));
				SetupResult.For(conn.ServerURL).Return("");
			}).Verify(delegate {
				var docs = new[] {new SampleDoc()};
				var cmd = new AddCommand<SampleDoc>(docs);
				cmd.Execute(conn);
			});
		}

		[Test]
		public void ShouldntAlterOriginalServerUrl() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			var docs = new[] {new SampleDoc()};
			var cmd = new AddCommand<SampleDoc>(docs);
			cmd.Execute(conn);
		}

		[Test]
		public void SupportsDocumentWithStringCollection() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				conn.Post("/update",
				          "<add><doc><field name=\"coll\">one</field><field name=\"coll\">two</field></doc></add>");
				LastCall.Repeat.Once().Do(new Writer(delegate(string ignored, string s) {
					Console.WriteLine(s);
					return null;
				}));
				SetupResult.For(conn.ServerURL).Return("");
			}).Verify(delegate {
				var docs = new[] {new TestDocWithCollections()};
				var cmd = new AddCommand<TestDocWithCollections>(docs);
				cmd.Execute(conn);
			});
		}
	}
}