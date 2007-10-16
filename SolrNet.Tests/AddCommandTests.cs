using System;
using NUnit.Framework;
using Rhino.Mocks;

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

		public delegate string Writer(string ignored, string s);

		[Test]
		public void Execute() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			conn.Post("/update", "<add><doc><field name=\"Id\">id</field><field name=\"Flower\">23,5</field></doc></add>");			
			LastCall.Repeat.Once().Do(new Writer(delegate(string ignored, string s) {
			                                                       	Console.WriteLine(s);
			                                                       	return null;
			                                                       }));
			SetupResult.For(conn.ServerURL).Return("");
			mocks.ReplayAll();
			SampleDoc[] docs = new SampleDoc[] {new SampleDoc()};
			AddCommand<SampleDoc> cmd = new AddCommand<SampleDoc>(docs);
			cmd.Execute(conn);
			mocks.VerifyAll();
		}

		[Test]
		public void ShouldntAlterOriginalServerUrl() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			SampleDoc[] docs = new SampleDoc[] { new SampleDoc() };
			AddCommand<SampleDoc> cmd = new AddCommand<SampleDoc>(docs);
			cmd.Execute(conn);
		}
	}
}