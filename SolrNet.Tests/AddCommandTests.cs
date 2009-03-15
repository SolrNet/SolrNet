#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Commands;
using SolrNet.Impl;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Mapping;

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
		    var docSerializer = new SolrDocumentSerializer<SampleDoc>(new AttributesMappingManager(), new DefaultFieldSerializer());
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
				var cmd = new AddCommand<SampleDoc>(docs, docSerializer);
				cmd.Execute(conn);
			});
		}

		[Test]
		public void ShouldntAlterOriginalServerUrl() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
            var docSerializer = new SolrDocumentSerializer<SampleDoc>(new AttributesMappingManager(), new DefaultFieldSerializer());
			var cmd = new AddCommand<SampleDoc>(new[] {new SampleDoc()}, docSerializer);
			cmd.Execute(conn);
		}

		[Test]
		public void SupportsDocumentWithStringCollection() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
            var docSerializer = new SolrDocumentSerializer<TestDocWithCollections>(new AttributesMappingManager(), new DefaultFieldSerializer());
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
				var cmd = new AddCommand<TestDocWithCollections>(docs, docSerializer);
				cmd.Execute(conn);
			});
		}
	}
}