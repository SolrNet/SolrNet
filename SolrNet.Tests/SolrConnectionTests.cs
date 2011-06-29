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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using HttpWebAdapters;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Exceptions;
using SolrNet.Impl;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrConnectionTests {
        private const string solrURL = "http://localhost:8983/solr";

		[Test]
		[Category("Integration")]
		[Ignore]
		public void ActualConnection() {
            var conn = new SolrConnection(solrURL) { HttpWebRequestFactory = new HttpWebRequestFactory() };
			var p = new Dictionary<string, string>();
			p["version"] = "2.1";
			p["indent"] = "on";
			p["q"] = "+video +price:[* TO 400]";
			Console.WriteLine(conn.Get("/select/", p));
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void ActualConnectionWithException() {
            var conn = new SolrConnection(solrURL);
			var p = new Dictionary<string, string>();
			p["version"] = "2.1";
			p["indent"] = "on";
			p["q"] = "idq:123";
            try {
                conn.Get("/select/", p);
                Assert.Fail("Should have thrown");
            } catch (SolrConnectionException e) {
                Console.WriteLine(e);
                Console.WriteLine(e.Url);
            }
		}

		[Test]
		public void Get() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			var response = mocks.DynamicMock<IHttpWebResponse>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
                Expect.Call(request.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
                Expect.Call(response.ContentEncoding)
                    .Repeat.Any()
                    .Return(string.Empty);
			    Expect.Call(response.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
				Expect.Call(request.GetResponse())
                    .Repeat.Once()
                    .Return(response);
				Expect.Call(response.GetResponseStream())
                    .Repeat.Once()
                    .Return(new MemoryStream());
			}).Verify(delegate {
				var conn = new SolrConnection("https://pepe"){ HttpWebRequestFactory = reqFactory };
				conn.Get("", new Dictionary<string, string>());
			});
		}

        [Test]
        public void Get_Compressed_Gzip()
        {
            var mocks = new MockRepository();
            var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
            var request = mocks.DynamicMock<IHttpWebRequest>();
            var response = mocks.DynamicMock<IHttpWebResponse>();
            With.Mocks(mocks).Expecting(delegate
            {
                Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
                Expect.Call(request.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
                Expect.Call(response.ContentEncoding)
                    .Repeat.Any()
                    .Return("gzip");
                Expect.Call(response.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
                Expect.Call(request.GetResponse())
                    .Repeat.Once()
                    .Return(response);
                Expect.Call(response.GetResponseStream())
                    .Repeat.Once()
                    .Return(CompressionUtils.GzipCompressStream("Testing compression"));
            }).Verify(delegate {
                var conn = new SolrConnection("http://localhost") { HttpWebRequestFactory = reqFactory };
                Assert.AreEqual("Testing compression", conn.Get("", new Dictionary<string, string>()));
            });
        }

        [Test]
        public void Get_Compressed_Deflate()
        {
            var mocks = new MockRepository();
            var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
            var request = mocks.DynamicMock<IHttpWebRequest>();
            var response = mocks.DynamicMock<IHttpWebResponse>();
            With.Mocks(mocks).Expecting(delegate {
                Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
                Expect.Call(request.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
                Expect.Call(response.ContentEncoding)
                    .Repeat.Any()
                    .Return("deflate");
                Expect.Call(response.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
                Expect.Call(request.GetResponse())
                    .Repeat.Once()
                    .Return(response);
                Expect.Call(response.GetResponseStream())
                    .Repeat.Once()
                    .Return(CompressionUtils.DeflateCompressStream("Testing compression"));
            }).Verify(delegate {
                var conn = new SolrConnection("http://localhost") { HttpWebRequestFactory = reqFactory };
                Assert.AreEqual("Testing compression", conn.Get("", new Dictionary<string, string>()));
            });
        }

		[Test]
		public void GetWithNullParameters_ShouldAcceptNull() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			var response = mocks.DynamicMock<IHttpWebResponse>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
                Expect.Call(request.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
                Expect.Call(response.ContentEncoding)
                    .Repeat.Any()
                    .Return(string.Empty);
                Expect.Call(response.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
				Expect.Call(request.GetResponse())
                    .Repeat.Once()
                    .Return(response);
				Expect.Call(response.GetResponseStream())
                    .Repeat.Once()
                    .Return(new MemoryStream());
			}).Verify(delegate {
                var conn = new SolrConnection("https://pepe") { HttpWebRequestFactory = reqFactory };
                conn.Get("", new Dictionary<string, string>());
			});
		}

		[Test]
		[ExpectedException(typeof (SolrConnectionException))]
		public void InvalidHostGet_ShouldThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
                Expect.Call(request.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
				Expect.Call(request.GetResponse())
                    .Repeat.Once()
                    .Throw(new WebException());
			}).Verify(delegate {
				var conn = new SolrConnection("http://lalala:12345") { HttpWebRequestFactory = reqFactory };
				conn.Get("", new Dictionary<string, string>());
			});
		}

		[Test]
		[ExpectedException(typeof (SolrConnectionException))]
		public void InvalidHostPost_ShouldThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			With.Mocks(mocks).Expecting(delegate {
                var uri = new Uri("http://lalala:12345/update");
				Expect.Call(request.GetRequestStream())
                    .Repeat.Once()
                    .Throw(new WebException());
                Expect.Call(request.RequestUri).Repeat.Once().Return(uri);
				Expect.Call(reqFactory.Create(uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
			}).Verify(delegate {
				var conn = new SolrConnection("http://lalala:12345") { HttpWebRequestFactory = reqFactory };
				conn.Post("/update", "");
			});
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void InvalidUrl_ShouldThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
            new SolrConnection("http:/locl") { HttpWebRequestFactory = reqFactory };
		}

		[Test]
		public void Post() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			var response = mocks.DynamicMock<IHttpWebResponse>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(request.GetRequestStream())
                    .Repeat.Once()
                    .Return(new MemoryStream());
				Expect.Call(reqFactory.Create(new Uri("https://pepe/?version=2.2")))
                    .Repeat.Once()
                    .Return(request);
				request.Method = HttpWebRequestMethod.POST;
				LastCall.On(request).Repeat.Once();
				Expect.Call(request.GetResponse())
                    .Repeat.Once()
                    .Return(response);
                Expect.Call(response.Headers)
                    .Repeat.Any()
                    .Return(new WebHeaderCollection());
				Expect.Call(response.GetResponseStream())
                    .Repeat.Once()
                    .Return(new MemoryStream());
			}).Verify(delegate {
                var conn = new SolrConnection("https://pepe") { HttpWebRequestFactory = reqFactory };
				conn.Post("", "");
			});
		}

		[Test]
		public void UrlHttp_ShouldntThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
            new SolrConnection("http://pepe") { HttpWebRequestFactory = reqFactory };
		}

		[Test]
		public void UrlHttps_ShouldntThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
            new SolrConnection("https://pepe") { HttpWebRequestFactory = reqFactory };
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void UrlNotHttp_ShouldThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.StrictMock<IHttpWebRequestFactory>();
            new SolrConnection("ftp://pepe") { HttpWebRequestFactory = reqFactory };
		}

        [Test]
        [Ignore]
        public void Cache_mocked() {
            var conn = new SolrConnection(solrURL);
            var cache = MockRepository.GenerateMock<ISolrCache>();
            cache.Expect(x => x["http://localhost:8983/solr/select/?q=*:*"])
                .Repeat.Once()
                .Return(null);
            cache.Expect(x => x.Add(null)).Repeat.Once();
            conn.Cache = cache;
            var response1 = conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });
            var response2 = conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });
        }

        [Test]
        [Ignore]
        public void Cache() {
            var conn = new SolrConnection(solrURL);
            var response1 = conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });
            var response2 = conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });
        }

        [Test]
        [Ignore]
        public void Cache_performance() {
            var conn = new SolrConnection(solrURL) {
                Cache = new HttpRuntimeCache(),
            };
            TestCache(conn);
        }

        [Test]
        [Ignore]
        public void NoCache_performance() {
            var conn = new SolrConnection(solrURL) {
                Cache = new NullCache(),
            };
            TestCache(conn);
        }

        public void TestCache(ISolrConnection conn) {
            foreach (var i in Enumerable.Range(0, 1000)) {
                conn.Get("/select/", new Dictionary<string, string> {
                    {"q", "*:*"},
                });
            }
        }
	}
}