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
using Xunit;
using Moroco;
using SolrNet.Exceptions;
using SolrNet.Impl;
using Xunit.Abstractions;

namespace SolrNet.Tests {
	
	public class SolrConnectionTests {
        private readonly ITestOutputHelper testOutputHelper;

        public SolrConnectionTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        private const string solrURL = "http://localhost:8983/solr/techproducts";

        [Fact(Skip = "unknown reason")]
        [Trait("Category", "Integration")]
		public void ActualConnection() {
            var conn = new SolrConnection(solrURL) { HttpWebRequestFactory = new HttpWebRequestFactory() };
			var p = new Dictionary<string, string>();
			p["version"] = "2.1";
			p["indent"] = "on";
			p["q"] = "+video +price:[* TO 400]";
			testOutputHelper.WriteLine(conn.Get("/select/", p));
		}

		[Trait("Category", "Integration")]
        [Fact(Skip = "unknown reason")]
        public void ActualConnectionWithException() {
            var conn = new SolrConnection(solrURL);
			var p = new Dictionary<string, string>();
			p["version"] = "2.1";
			p["indent"] = "on";
			p["q"] = "idq:123";
            try {
                conn.Get("/select/", p);
                Assert.True(false, "Should have thrown");
            } catch (SolrConnectionException e) {
                testOutputHelper.WriteLine(e.ToString());
                testOutputHelper.WriteLine(e.Url);
            }
		}

		[Fact]
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
            Assert.Equal("hello world", r);
		}

        [Fact]
        public void WithWtXml_Get()
        {
            var expected = "https://pepe/?wt=xml&version=2.2";

            var cache = new Mocks.MSolrCache();
            cache.get += url => new SolrCacheEntity(url, "etag", "hello xml world");
            cache.add &= x => x.Stub();

            var headers = new WebHeaderCollection
            {
                {HttpResponseHeader.ETag, "etag"}, {HttpResponseHeader.CacheControl, "cache"}
            };
            var response = new Mocks.HttpWebResponse
            {
                dispose = () => { },
                headers = () => headers,
                getResponseStream = () => new MemoryStream(Encoding.UTF8.GetBytes("hello xml world")),
            };

            var request = new Mocks.HttpWebRequest
            {
                getResponse = () => response,
                Headers = new WebHeaderCollection()
            };

            var reqFactory = new Mocks.HttpWebRequestFactory
            {
                create = _ => request
            };
            var conn = new SolrConnection("https://pepe")
            {
                HttpWebRequestFactory = reqFactory,
                Cache = cache
            };

            var r = conn.Get("", new Dictionary<string, string>( ));
            var actual = conn.Cache[expected];
            Assert.Equal("hello xml world", r);
            Assert.Equal(actual.Url, expected);
        }

        [Fact]
        public void WithWtJson_Get()
        {
            var expected = "https://pepe/?wt=json&version=2.2";

            var cache = new Mocks.MSolrCache();
            cache.get += url => new SolrCacheEntity(url, "", "");
            cache.add &= x => x.Stub();

            var headers = new WebHeaderCollection
            {
                {HttpResponseHeader.ETag, "etag"}, {HttpResponseHeader.CacheControl, "cache"}
            };
            var response = new Mocks.HttpWebResponse
            {
                dispose = () => { },
                headers = () => headers,
                getResponseStream = () => new MemoryStream(Encoding.UTF8.GetBytes("hello json world")),
            };

            var request = new Mocks.HttpWebRequest
            {
                getResponse = () => response, 
                Headers = new WebHeaderCollection()
            };

            var reqFactory = new Mocks.HttpWebRequestFactory
            {
                create = _ => request
            };
            var conn = new SolrConnection("https://pepe")
            {
                HttpWebRequestFactory = reqFactory,
                Cache = cache
            };

            var r = conn.Get("", new Dictionary<string, string> { {"wt", "json"} });
            var actual = conn.Cache[expected];
            Assert.Equal("hello json world", r);
            Assert.Equal(actual.Url, expected);
        }

        [Fact]
		public void InvalidHostGet_ShouldThrowException() {
		    var reqFactory = new Mocks.HttpWebRequestFactory {
		        create = _ => new Mocks.HttpWebRequest {
		            getResponse = () => { throw new WebException();}
		        }
		    };
            var conn = new SolrConnection("http://lalala:12345") { HttpWebRequestFactory = reqFactory };
            Assert.Throws<SolrConnectionException>(() => conn.Get("", new Dictionary<string, string>()));
		}

		[Fact]
		public void InvalidHostPost_ShouldThrowException() {
		    var reqFactory = new Mocks.HttpWebRequestFactory {
		        create = _ => new Mocks.HttpWebRequest {
		            getRequestStream = () => { throw new WebException(); },
                    requestUri = () => new Uri("http://lalala:12345/update"),
		        }
		    };
            var conn = new SolrConnection("http://lalala:12345") { HttpWebRequestFactory = reqFactory };
            Assert.Throws<SolrConnectionException>(() => conn.Post("/update", ""));
		}

		[Fact]
		public void InvalidUrl_ShouldThrowException() {
            Assert.Throws<InvalidURLException>(() => new SolrConnection("http:/locl"));
		}

		[Fact]
		public void UrlHttp_ShouldntThrowException() {
		    new SolrConnection("http://pepe");
		}

		[Fact]
		public void UrlHttps_ShouldntThrowException() {
		    new SolrConnection("https://pepe");
		}

		[Fact]
		public void UrlNotHttp_ShouldThrowException() {
		Assert.Throws< InvalidURLException> (()=>   new SolrConnection("ftp://pepe"));
		}

        [Fact(Skip = "need to mock WebException!")]
        public void Cache_mocked() {
            var cache = new Mocks.MSolrCache();
            cache.get += url => {
                Assert.Equal("http://localhost:8983/solr/techproducts/select/?q=*:*&version=2.2", url);
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

        [Fact(Skip = "unknown reason")]
        public void Cache() {
            var conn = new SolrConnection(solrURL);
            conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });
            conn.Get("/select/", new Dictionary<string, string> {
                {"q", "*:*"},
            });
        }

      

        [Fact(Skip = "unknown reason")]
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
