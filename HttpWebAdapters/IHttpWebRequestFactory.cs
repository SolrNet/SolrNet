using System;

namespace SolrNet.Tests {
	public interface IHttpWebRequestFactory {
		IHttpWebRequest Create(string url);
		IHttpWebRequest Create(Uri url);
	}
}