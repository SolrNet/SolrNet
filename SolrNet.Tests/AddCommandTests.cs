#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Commands;
using SolrNet.Impl;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Mapping;

namespace SolrNet.Tests {
	[TestFixture]
	public class AddCommandTests {
		public class SampleDoc  {
			[SolrField]
			public string Id {
				get { return "id"; }
			}

			[SolrField("Flower")]
			public decimal caca {
				get { return 23.5m; }
			}
		}

		public class TestDocWithCollections  {
			[SolrField]
			public ICollection<string> coll {
				get { return new[] {"one", "two"}; }
			}
		}

        public class TestDocWithString {
            [SolrField]
            public string Desc { get; set; }
        }

        public class TestDocWithFieldBoost
        {
            [SolrField(Boost = 20)]
            public string SimpleBoost
            {
                get { return "simple"; }
            }

            [SolrField("nameandboost", Boost = 20)]
            public string NameAndBoost
            {
                get { return "boost"; }
            }
        }

		public delegate string Writer(string ignored, string s);

		[Test]
		public void Execute() {
			var mocks = new MockRepository();
			var conn = mocks.StrictMock<ISolrConnection>();
		    var docSerializer = new SolrDocumentSerializer<SampleDoc>(new AttributesMappingManager(), new DefaultFieldSerializer());
			With.Mocks(mocks).Expecting(() => {
			    conn.Post("/update",
			              "<add><doc><field name=\"Id\">id</field><field name=\"Flower\">23.5</field></doc></add>");
			    LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s) {
			        Console.WriteLine(s);
			        return null;
			    }));
                Expect.On(conn).Call(conn.ServerURL).Repeat.Any().Return("");
			}).Verify(() => {
			    var docs = new[] {new SampleDoc()};
			    var cmd = new AddCommand<SampleDoc>(docs, docSerializer);
			    cmd.Execute(conn);
			});
		}

        [Test]
        public void DocumentBoost() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var docSerializer = new SolrDocumentSerializer<TestDocWithString>(new AttributesMappingManager(), new DefaultFieldSerializer());
            With.Mocks(mocks).Expecting(() => {
                conn.Post("/update",
                          "<add><doc boost=\"2.1\" /></add>");
                LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s) {
                    Console.WriteLine(s);
                    return null;
                }));
                Expect.On(conn).Call(conn.ServerURL).Repeat.Any().Return("");
            }).Verify(() => {
                var docs = new[] { new KeyValuePair<TestDocWithString, double?>(new TestDocWithString(), 2.1) };
                var cmd = new AddCommand<TestDocWithString>(docs, docSerializer);
                cmd.Execute(conn);
            });
        }

        [Test]
        public void FieldBoost()
        {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var docSerializer = new SolrDocumentSerializer<TestDocWithFieldBoost>(new AttributesMappingManager(), new DefaultFieldSerializer());
            With.Mocks(mocks).Expecting(() =>
            {
                conn.Post("/update",
                          "<add><doc><field name=\"SimpleBoost\" boost=\"20\">simple</field><field name=\"nameandboost\" boost=\"20\">boost</field></doc></add>");
                LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s)
                {
                    Console.WriteLine(s);
                    return null;
                }));
                Expect.On(conn).Call(conn.ServerURL).Repeat.Any().Return("");
            }).Verify(() =>
            {
                var docs = new[] { new TestDocWithFieldBoost() };
                var cmd = new AddCommand<TestDocWithFieldBoost>(docs, docSerializer);
                cmd.Execute(conn);
            });
        }

		[Test]
		public void ShouldntAlterOriginalServerUrl() {
			var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var docSerializer = new SolrDocumentSerializer<SampleDoc>(new AttributesMappingManager(), new DefaultFieldSerializer());
			var cmd = new AddCommand<SampleDoc>(new[] {new SampleDoc()}, docSerializer);
			cmd.Execute(conn);
		}

		[Test]
		public void SupportsDocumentWithStringCollection() {
			var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var docSerializer = new SolrDocumentSerializer<TestDocWithCollections>(new AttributesMappingManager(), new DefaultFieldSerializer());
			With.Mocks(mocks).Expecting(() => {
			    conn.Post("/update",
			              "<add><doc><field name=\"coll\">one</field><field name=\"coll\">two</field></doc></add>");
			    LastCall.On(conn).Repeat.Once().Do(new Writer(delegate(string ignored, string s) {
			        Console.WriteLine(s);
			        return null;
			    }));
                Expect.On(conn).Call(conn.ServerURL).Repeat.Any().Return("");
			}).Verify(() => {
			    var docs = new[] {new TestDocWithCollections()};
			    var cmd = new AddCommand<TestDocWithCollections>(docs, docSerializer);
			    cmd.Execute(conn);
			});
		}

        [Test]
        public void RemovesControlCharactersFromXML() {
            var docSerializer = new SolrDocumentSerializer<TestDocWithString>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var docs = new[] { new TestDocWithString { Desc = "control" + (char)0x7 + (char)0x1F + (char)0xFFFE + (char)0xFFFF + (char)0xFFF4  } };
		    var cmd = new AddCommand<TestDocWithString>(docs, docSerializer);
            var xml = cmd.ConvertToXml();
            xml = cmd.RemoveControlCharacters(xml);
            Console.WriteLine(xml);
            Assert.DoesNotContain(xml, "&#x7;");
            Assert.DoesNotContain(xml, "&#x1;");
            Assert.DoesNotContain(xml, "&#x1F;");
            Assert.DoesNotContain(xml, "&#xFFFE;");
        }

        [Test]
        public void RemoveControlCharacters() {
            var mocks = new MockRepository();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<TestDocWithString>>();
            var docs = new[] { new TestDocWithString() };
            var cmd = new AddCommand<TestDocWithString>(docs, docSerializer);
            var xml = cmd.RemoveControlCharacters("control &#x7; &#x1; &#x9; &#x1F; &#xFFFE;");
            Assert.DoesNotContain(xml, "&#x7;");
            Assert.DoesNotContain(xml, "&#x1;");
            Assert.DoesNotContain(xml, "&#x1F;");
            Assert.DoesNotContain(xml, "&#xFFFE;");
            Assert.Contains(xml, "&#x9;");
        }
	}
}