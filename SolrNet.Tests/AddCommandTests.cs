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

		public delegate string Writer(string s);

		[Test]
		public void execute() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			conn.Post(null);
			LastCall.IgnoreArguments().Repeat.Once().Do(new Writer(delegate(string s) {
			                                                       	Console.WriteLine(s);
			                                                       	return null;
			                                                       }));
			mocks.ReplayAll();
			SampleDoc[] docs = new SampleDoc[] {new SampleDoc()};
			AddCommand<SampleDoc> cmd = new AddCommand<SampleDoc>(docs);
			cmd.Execute(conn);
			mocks.VerifyAll();
		}

	}
}