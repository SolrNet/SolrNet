using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HttpWebAdapters;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrConnectionTests {
		[Test]
		[Category("Integration")]
		[Ignore]
		public void ActualConnectionTest() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			IDictionary<string, string> p = new Dictionary<string, string>();
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
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			IDictionary<string, string> p = new Dictionary<string, string>();
			p["version"] = "2.1";
			p["indent"] = "on";
			p["q"] = "idq:123";
			Console.WriteLine(conn.Get("/select/", p));
		}

		[Test]
		public void get() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();
			Expect.Call(reqFactory.Create(new UriBuilder().Uri)).IgnoreArguments().Repeat.Once().Return(request);
			IHttpWebResponse response = mocks.DynamicMock<IHttpWebResponse>();
			Expect.Call(request.GetResponse()).Repeat.Once().Return(response);
			Expect.Call(response.GetResponseStream()).Repeat.Once().Return(new MemoryStream());
			mocks.ReplayAll();
			ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
			conn.Get("", new Dictionary<string, string>());
		}

		[Test]
		public void GetWithNullParameters_ShouldAcceptNull() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();
			IHttpWebResponse response = mocks.DynamicMock<IHttpWebResponse>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri)).IgnoreArguments().Repeat.Once().Return(request);
				Expect.Call(request.GetResponse()).Repeat.Once().Return(response);
				Expect.Call(response.GetResponseStream()).Repeat.Once().Return(new MemoryStream());
			}).Verify(delegate {
				ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
				conn.Get("", null);
			});
		}

		[Test]
		[ExpectedException(typeof (SolrConnectionException))]
		public void InvalidHostGet_ShouldThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri)).IgnoreArguments().Repeat.Once().Return(request);
				Expect.Call(request.GetResponse()).Repeat.Once().Throw(new WebException());
			}).Verify(delegate {
				ISolrConnection conn = new SolrConnection("http://lalala:12345", reqFactory);
				conn.Get("", null);
			});
		}

		[Test]
		[ExpectedException(typeof (SolrConnectionException))]
		public void InvalidHostPost_ShouldThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(request.GetRequestStream()).Repeat.Once().Throw(new WebException());
				Expect.Call(reqFactory.Create(new Uri("http://lalala:12345/update"))).IgnoreArguments().Repeat.Once().Return(request);
			}).Verify(delegate {
				ISolrConnection conn = new SolrConnection("http://lalala:12345", reqFactory);
				conn.Post("/update", "");
			});
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void InvalidUrl_ShouldThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			ISolrConnection conn = new SolrConnection("http:/locl", reqFactory);
		}

		[Test]
		public void Post() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();
			IHttpWebResponse response = mocks.DynamicMock<IHttpWebResponse>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(request.GetRequestStream()).Repeat.Once().Return(new MemoryStream());
				Expect.Call(reqFactory.Create(new Uri("https://pepe"))).Repeat.Once().Return(request);
				request.Method = HttpWebRequestMethod.POST;
				LastCall.Repeat.Once();
				Expect.Call(request.GetResponse()).Repeat.Once().Return(response);
				Expect.Call(response.GetResponseStream()).Repeat.Once().Return(new MemoryStream());
			}).Verify(delegate {
				ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
				conn.Post("", "");
			});
		}

		[Test]
		[ExpectedException(typeof (InvalidFieldException))]
		public void UndefinedFieldQueryError_ShouldThrow() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(reqFactory.Create(new UriBuilder().Uri)).IgnoreArguments().Repeat.Once().Return(request);
				WebResponseStub r = new WebResponseStub();
				r.StatusCode = HttpStatusCode.BadRequest;
				Expect.Call(request.GetResponse()).Repeat.Once()
					.Throw(new WebException("(400) Bad Request", new ApplicationException(), WebExceptionStatus.ProtocolError, r));
			}).Verify(delegate {
				ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
				conn.Get("", new Dictionary<string, string>());
			});
		}

		[Test]
		public void UrlHttp_ShouldntThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			ISolrConnection conn = new SolrConnection("http://pepe", reqFactory);
		}

		[Test]
		public void UrlHttps_ShouldntThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void UrlNotHttp_ShouldThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			ISolrConnection conn = new SolrConnection("ftp://pepe", reqFactory);
		}
	}
}