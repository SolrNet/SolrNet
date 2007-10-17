using System;
using System.Net;
using HttpWebAdapters.Adapters;

namespace HttpWebAdapters {
	public class HttpWebRequestFactory : IHttpWebRequestFactory {
		public IHttpWebRequest Create(string url) {
			return new HttpWebRequestAdapter((HttpWebRequest) WebRequest.Create(url));
		}

		public IHttpWebRequest Create(Uri url) {
			return new HttpWebRequestAdapter((HttpWebRequest) WebRequest.Create(url));
		}
	}
}