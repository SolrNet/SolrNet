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
using Xunit;
using Moroco;
using SolrNet.Attributes;
using SolrNet.Commands;
using SolrNet.Impl;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;
using Xunit.Abstractions;

namespace SolrNet.Tests {
	
	public class AddCommandTests {
        private readonly ITestOutputHelper testOutputHelper;

        public AddCommandTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

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

		[Fact]
		public void Execute() {
		    var conn = new Mocks.MSolrConnection();
		    conn.post += (url, content) => {
		        Assert.Equal("/update", url);
		        Assert.Equal("<add><doc><field name=\"Id\">id</field><field name=\"Flower\">23.5</field></doc></add>", content);
		        testOutputHelper.WriteLine(content);
		        return null;
		    };
		    var docSerializer = new SolrDocumentSerializer<SampleDoc>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var docs = new[] {
                new KeyValuePair<SampleDoc, double?>(new SampleDoc(), null), 
			};
            var cmd = new AddCommand<SampleDoc>(docs, docSerializer, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void DocumentBoost() {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<add><doc boost=\"2.1\" /></add>", content);
                testOutputHelper.WriteLine(content);
                return null;
            };
            var docSerializer = new SolrDocumentSerializer<TestDocWithString>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var docs = new[] { new KeyValuePair<TestDocWithString, double?>(new TestDocWithString(), 2.1) };
            var cmd = new AddCommand<TestDocWithString>(docs, docSerializer, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void DocumentAddParametersCommitWithinSpecified() {
            var docSerializer = new SolrDocumentSerializer<TestDocWithString>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var conn = new Mocks.MSolrConnection();
            conn.post = conn.post
                .Args("/update", "<add commitWithin=\"1000\"><doc boost=\"2.1\" /></add>");
            var docs = new[] { new KeyValuePair<TestDocWithString, double?>(new TestDocWithString(), 2.1) };
            var parameters = new AddParameters { CommitWithin = 1000 };
            var cmd = new AddCommand<TestDocWithString>(docs, docSerializer, parameters);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void DocumentAddParametersOverwriteSpecifiedTrue() {
            var docSerializer = new SolrDocumentSerializer<TestDocWithString>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var conn = new Mocks.MSolrConnection();
            conn.post = conn.post
                .Args("/update", "<add overwrite=\"true\"><doc boost=\"2.1\" /></add>");
            var docs = new[] { new KeyValuePair<TestDocWithString, double?>(new TestDocWithString(), 2.1) };
            var parameters = new AddParameters { Overwrite = true };
            var cmd = new AddCommand<TestDocWithString>(docs, docSerializer, parameters);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void DocumentAddParametersOverwriteSpecifiedFalse() {
            var docSerializer = new SolrDocumentSerializer<TestDocWithString>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var conn = new Mocks.MSolrConnection();
            conn.post = conn.post
                .Args("/update", "<add overwrite=\"false\"><doc boost=\"2.1\" /></add>");
            var docs = new[] { new KeyValuePair<TestDocWithString, double?>(new TestDocWithString(), 2.1) };
            var parameters = new AddParameters { Overwrite = false };
            var cmd = new AddCommand<TestDocWithString>(docs, docSerializer, parameters);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void FieldBoost() {
            var docSerializer = new SolrDocumentSerializer<TestDocWithFieldBoost>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var conn = new Mocks.MSolrConnection();
            conn.post = conn.post
                .Args("/update", "<add><doc><field name=\"SimpleBoost\" boost=\"20\">simple</field><field name=\"nameandboost\" boost=\"20\">boost</field></doc></add>");
            var docs = new[] { new KeyValuePair<TestDocWithFieldBoost, double?>(new TestDocWithFieldBoost(), null) };
            var cmd = new AddCommand<TestDocWithFieldBoost>(docs, docSerializer, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

		[Fact]
		public void SupportsDocumentWithStringCollection() {
            var docSerializer = new SolrDocumentSerializer<TestDocWithCollections>(new AttributesMappingManager(), new DefaultFieldSerializer());
		    var conn = new Mocks.MSolrConnection();
            conn.post = conn.post
                .Args("/update", "<add><doc><field name=\"coll\">one</field><field name=\"coll\">two</field></doc></add>");
            var docs = new[] {
                new KeyValuePair<TestDocWithCollections, double?>(new TestDocWithCollections(), null), 
            };
            var cmd = new AddCommand<TestDocWithCollections>(docs, docSerializer, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
		}

        [Fact]
        public void RemovesControlCharactersFromXML() {
            var docSerializer = new SolrDocumentSerializer<TestDocWithString>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var doc = new TestDocWithString { Desc = "control" + (char)0x7 + (char)0x1F + (char)0xFFFE + (char)0xFFFF + (char)0xFFF4  };
            var docs = new[] {new KeyValuePair<TestDocWithString, double?>(doc, null),  };
		    var cmd = new AddCommand<TestDocWithString>(docs, docSerializer, null);
            var xml = cmd.ConvertToXml();
            xml = SolrDocumentSerializer<object>.RemoveControlCharacters(xml);
            //Console.WriteLine(xml);
            Assert.DoesNotContain(xml, "&#x7;");
            Assert.DoesNotContain(xml, "&#x1;");
            Assert.DoesNotContain(xml, "&#x1F;");
            Assert.DoesNotContain(xml, "&#xFFFE;");
        }

        [Fact]
        public void RemoveControlCharacters() {
            var xml = SolrDocumentSerializer<object>.RemoveControlCharacters("control " + (char)1);
            Assert.DoesNotContain((char)1, xml);
        }
	}
}
