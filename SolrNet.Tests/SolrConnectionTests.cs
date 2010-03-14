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
using System.Linq;
using System.Net;
using HttpWebAdapters;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Exceptions;
using SolrNet.Impl;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrConnectionTests {
        private const string solrURL = "http://localhost:8983/solr";

		[Test]
		[Category("Integration")]
		[Ignore]
		public void ActualConnectionTest() {
			var conn = new SolrConnection(solrURL, new HttpWebRequestFactory());
			var p = new Dictionary<string, string>();
			p["version"] = "2.1";
			p["indent"] = "on";
			p["q"] = "+video +price:[* TO 400]";
			Console.WriteLine(conn.Get("/select/", p));
		}

		[Test]
		[Category("Integration")]
		[ExpectedException(typeof (InvalidFieldException))]
		[Ignore]
		public void ActualInvalidFieldException() {
            var conn = new SolrConnection(solrURL, new HttpWebRequestFactory());
			var p = new Dictionary<string, string>();
			p["version"] = "2.1";
			p["indent"] = "on";
			p["q"] = "idq:123";
			Console.WriteLine(conn.Get("/select/", p));
		}

		[Test]
		public void Get() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			var response = mocks.DynamicMock<IHttpWebResponse>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
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
				var conn = new SolrConnection("https://pepe", reqFactory);
				conn.Get("", new Dictionary<string, string>());
			});
		}

		[Test]
		public void GetWithNullParameters_ShouldAcceptNull() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			var response = mocks.DynamicMock<IHttpWebResponse>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
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
				var conn = new SolrConnection("https://pepe", reqFactory);
                conn.Get("", new Dictionary<string, string>());
			});
		}

		[Test]
		[ExpectedException(typeof (SolrConnectionException))]
		public void InvalidHostGet_ShouldThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
				Expect.Call(request.GetResponse())
                    .Repeat.Once()
                    .Throw(new WebException());
			}).Verify(delegate {
				var conn = new SolrConnection("http://lalala:12345", reqFactory);
				conn.Get("", new Dictionary<string, string>());
			});
		}

		[Test]
		[ExpectedException(typeof (SolrConnectionException))]
		public void InvalidHostPost_ShouldThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(request.GetRequestStream())
                    .Repeat.Once()
                    .Throw(new WebException());
				Expect.Call(reqFactory.Create(new Uri("http://lalala:12345/update")))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
			}).Verify(delegate {
				var conn = new SolrConnection("http://lalala:12345", reqFactory);
				conn.Post("/update", "");
			});
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void InvalidUrl_ShouldThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			new SolrConnection("http:/locl", reqFactory);
		}

		[Test]
		public void Post() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			var response = mocks.DynamicMock<IHttpWebResponse>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(request.GetRequestStream())
                    .Repeat.Once()
                    .Return(new MemoryStream());
				Expect.Call(reqFactory.Create(new Uri("https://pepe")))
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
				var conn = new SolrConnection("https://pepe", reqFactory);
				conn.Post("", "");
			});
		}

		[Test]
		[ExpectedException(typeof (InvalidFieldException))]
		public void UndefinedFieldQueryError_ShouldThrow() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			var request = mocks.DynamicMock<IHttpWebRequest>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(request);
				var r = new WebResponseStub {StatusCode = HttpStatusCode.BadRequest};
				Expect.Call(request.GetResponse())
                    .Repeat.Once()
					.Throw(new WebException("(400) Bad Request", new ApplicationException(), WebExceptionStatus.ProtocolError, r));
			}).Verify(delegate {
				var conn = new SolrConnection("https://pepe", reqFactory);
				conn.Get("", new Dictionary<string, string>());
			});
		}

		[Test]
		public void UrlHttp_ShouldntThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			new SolrConnection("http://pepe", reqFactory);
		}

		[Test]
		public void UrlHttps_ShouldntThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			new SolrConnection("https://pepe", reqFactory);
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void UrlNotHttp_ShouldThrowException() {
			var mocks = new MockRepository();
			var reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			new SolrConnection("ftp://pepe", reqFactory);
		}

        [Test]
        [Ignore]
        public void Cache_mocked() {
            var conn = new SolrConnection(solrURL, new HttpWebRequestFactory());
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
            var conn = new SolrConnection(solrURL, new HttpWebRequestFactory());
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
            var conn = new SolrConnection(solrURL, new HttpWebRequestFactory()) {
                Cache = new HttpRuntimeCache(),
            };
            TestCache(conn);
        }

        [Test]
        [Ignore]
        public void NoCache_performance() {
            var conn = new SolrConnection(solrURL, new HttpWebRequestFactory()) {
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