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
using System.Text;
using HttpWebAdapters;
using MbUnit.Framework;
using Moroco;
using SolrNet.Exceptions;
using SolrNet.Impl;

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
		    var response = new Mocks.HttpWebResponse {
		        dispose = () => {},
                headers = () => new WebHeaderCollection(),
                getResponseStream = () => new MemoryStream(Encoding.UTF8.GetBytes("hello world")),
		    };
		    var request = new Mocks.HttpWebRequest {
		        getResponse = () => response
		    };
		    var reqFactory = new Mocks.HttpWebRequestFactory {
		        create = _ => request
		    };
            var conn = new SolrConnection("https://pepe") {
                HttpWebRequestFactory = reqFactory,
            };
		    var r = conn.Get("", new Dictionary<string, string>());
            Assert.AreEqual("hello world", r);
		}

		[Test]
		[ExpectedException(typeof (SolrConnectionException))]
		public void InvalidHostGet_ShouldThrowException() {
		    var reqFactory = new Mocks.HttpWebRequestFactory {
		        create = _ => new Mocks.HttpWebRequest {
		            getResponse = () => { throw new WebException();}
		        }
		    };
            var conn = new SolrConnection("http://lalala:12345") { HttpWebRequestFactory = reqFactory };
            conn.Get("", new Dictionary<string, string>());
		}

		[Test]
		[ExpectedException(typeof (SolrConnectionException))]
		public void InvalidHostPost_ShouldThrowException() {
		    var reqFactory = new Mocks.HttpWebRequestFactory {
		        create = _ => new Mocks.HttpWebRequest {
		            getRequestStream = () => { throw new WebException(); },
                    requestUri = () => new Uri("http://lalala:12345/update"),
		        }
		    };
            var conn = new SolrConnection("http://lalala:12345") { HttpWebRequestFactory = reqFactory };
            conn.Post("/update", "");
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void InvalidUrl_ShouldThrowException() {
		    new SolrConnection("http:/locl");
		}

		[Test]
		public void UrlHttp_ShouldntThrowException() {
		    new SolrConnection("http://pepe");
		}

		[Test]
		public void UrlHttps_ShouldntThrowException() {
		    new SolrConnection("https://pepe");
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void UrlNotHttp_ShouldThrowException() {
		    new SolrConnection("ftp://pepe");
		}

        [Test]
        [Ignore("need to mock WebException!")]
        public void Cache_mocked() {
            var cache = new Mocks.MSolrCache();
            cache.get += url => {
                Assert.AreEqual("http://localhost:8983/solr/select/?q=*:*&version=2.2", url);
                return new SolrCacheEntity(url, "", "");
            };
            cache.add &= x => x.Stub();

            var response = new Mocks.HttpWebResponse {
                dispose = () => {},
                headers = () => new WebHeaderCollection {
                    {HttpResponseHeader.ETag, "123"},
                },
                getResponseStream = () => new MemoryStream(),
            };
            var getResponseCalls = 0;
            var conn = new SolrConnection(solrURL) {
                Cache = cache,
                HttpWebRequestFactory = new Mocks.HttpWebRequestFactory {
                    create = _ => new Mocks.HttpWebRequest {
                        getResponse = () => {
                            getResponseCalls++;
                            if (getResponseCalls == 1)
                                return response;
                            throw new Exception();
                        },
                        Headers = new WebHeaderCollection(),
                    },
                }
            };

            conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });

            conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });
        }

        [Test]
        [Ignore]
        public void Cache() {
            var conn = new SolrConnection(solrURL);
            conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });
            conn.Get("/select/", new Dictionary<string, string> {
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